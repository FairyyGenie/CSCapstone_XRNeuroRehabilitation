using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Oculus.Interaction;
using OculusSampleFramework;
using Oculus.Interaction.HandGrab;
public class CollisionHandler : MonoBehaviour
{
    public AudioSource audio;
    public GameObject openLock;
    public GameObject playeropenLock;

    public GameObject lockhinge;
    public GameObject playerlockhinge;
    public GameObject pen;

    public bool task1done = false;
    public bool task2done = false;

    public string keyTag; // Tag of the key GameObject
    public string rotationTag; // Tag of the GameObject representing the rotation
    public GameObject boxWithScript; // The GameObject containing the script you want to activate
    public GameObject playerboxWithScript; // The GameObject containing the script you want to activate

    private bool collisionOccurred = false; // Flag to track if the collision occurred
    private bool playercollisionOccurred = false; // Flag to track if the collision occurred
    private bool lockOpened = false; // Flag to track if the lock has been opened
    private bool animelockOpened = false; // Flag to track if the lock has been opened
    public GameObject Touch;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("rotation"))
        {
            collisionOccurred = true;
        }
        else
        {
            collisionOccurred = false;
        }
        if (collision.gameObject.CompareTag("playerrotation"))
        {
            playercollisionOccurred = true;

        }
    }

    void Update()
    {
        if (animelockOpened == true)
        {
            openLock.SetActive(false);
            lockhinge.SetActive(true);
            animelockOpened = false;
        }

        // Check if a collision has occurred and the lock hasn't been opened yet
        if ((collisionOccurred ) || (playercollisionOccurred && !lockOpened))
        {
            // Check if the key's rotation angle meets the required angle
            float rotationAngle = transform.eulerAngles.z;

            if(rotationAngle < 10 && rotationAngle > 0)
            {
               
                OpenLock();

            }
        }
    }

    void OpenLock()
    {

        // Play audio, deactivate lock, and activate box script
        audio.Play();
        if (playercollisionOccurred)
        {
            playeropenLock.SetActive(true);
            playerlockhinge.SetActive(false);

            task1done = true;
            lockOpened = true;
            ActivateBoxScript();
        }
        else if (collisionOccurred)
        {
            openLock.SetActive(true);
            lockhinge.SetActive(false);
           
            animelockOpened = true;
            ActivateBoxScript();

        }

    }


    void ActivateBoxScript()
    {
        // Check if the lock has been opened
        if (animelockOpened)
        {
            // Check if the box has a script attached
            if (boxWithScript != null)
            {
                // Get the script component attached to the box
                MonoBehaviour script = boxWithScript.GetComponent<MonoBehaviour>();

                // Check if the script exists and is disabled
                if (script != null && !script.enabled)
                {
                    // Enable the script
                    script.enabled = true;
                    Debug.Log("Box script activated.");
                }
            }
        }
        if (lockOpened)
         {
            if (playerboxWithScript != null)
            {
                // Get the script component attached to the box
                MonoBehaviour script = playerboxWithScript.GetComponent<MonoBehaviour>();

                // Check if the script exists and is disabled
                if (script != null && !script.enabled)
                {
                    // Enable the script
                    script.enabled = true;
                    Debug.Log("Playback box script activated.");

                    Cube cube = Touch.GetComponent<Cube>();
                    task2done = cube.touched;
                    Debug.Log("TASK2 " + cube.touched);
                    //task2done = true;
                }
            }
        }
            
        }
    
}
