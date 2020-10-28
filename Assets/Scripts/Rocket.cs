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
            Thrust();
            Rotate();
        }
        
    }

    private void Rotate()
    {
        rb.freezeRotation = true;
        float rotSframe = rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotSframe);
        }
        else if (Input.GetKey(KeyCode.D))
        {   
            transform.Rotate(-Vector3.forward * rotSframe);
        }

        rb.freezeRotation = false;
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
                state = State.Transcending;
                Invoke("LoadNextScene", levelDelay);
                break;
            default:
                state = State.Dying;
                Invoke("LoadFirstScene", levelDelay);
                break;

        }
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float thrustFrame = thrust * Time.deltaTime;
            rb.AddRelativeForce(Vector3.up * thrustFrame);
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }
        else
        {
            audio.Stop();
        }
    }
}
