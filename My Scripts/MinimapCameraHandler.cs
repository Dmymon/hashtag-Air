using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MinimapCameraHandler : MonoBehaviour
{
    public GameObject player;
    public Camera myCam;
    // public AudioListener myAudioListener;
    NetworkIdentity myID;

    // Use this for initialization
    void Start()
    {
        myID = player.GetComponent<NetworkIdentity>();

        if (myID.isLocalPlayer)
        {
            //Only the client that owns this object executes this code
            if (myCam.enabled == false)
                myCam.enabled = true;

            //  if (myAudioListener.enabled == false)
            // myAudioListener.enabled = true;

        }
        else
        {
            myCam.enabled = false;
            // myAudioListener.enabled = false;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (!myID.isLocalPlayer)
            return;

        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}