using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rb;
    new AudioSource audio;
    [SerializeField] float thrust = 100f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float levelDelay = 1f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelLoad;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem succesParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] Scene currentScene;
    enum State {  Alive, Dying, Transcending }
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        
        Move();
    }

    private void Move()
    { 
        if(state!= State.Dying)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void RespondToRotateInput()
    {
        rb.angularVelocity = Vector3.zero;
        float rotSframe = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotSframe);
        }
        else if (Input.GetKey(KeyCode.D))
        {   
            transform.Rotate(-Vector3.forward * rotSframe);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Fuel":
                Debug.Log("fuel");
                break;
            case "LandingPad":
                StartNextLevel();
                break;
            default:
                StartDeathSequence();
                break;

        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audio.Stop();
        audio.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke(nameof(LoadFirstScene), levelDelay);
    }

    private void StartNextLevel()
    {
        state = State.Transcending;
        audio.Stop();
        audio.PlayOneShot(levelLoad);
        succesParticles.Play();
        Invoke(nameof(LoadNextScene), levelDelay);
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        currentScene = SceneManager.GetActiveScene();
        int nextLevel = currentScene.buildIndex + 1;
        int sceneIndex = SceneManager.sceneCountInBuildSettings;
        if (nextLevel ==  sceneIndex)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(nextLevel);
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audio.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustFrame = thrust * Time.deltaTime;
        rb.AddRelativeForce(Vector3.up * thrustFrame);
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
