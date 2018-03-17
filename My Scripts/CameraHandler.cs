using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
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
        if (!myCam.enabled)
            return;

         myCam.transform.LookAt(player.transform);

        //
        ///Make all Text elements rotate with camera
        //
        Text[] allMessagesInScene = FindObjectsOfType<Text>();

        foreach (Text text in allMessagesInScene)
        {
            if (text.tag.Equals("broadCastMessage") || text.tag.Equals("name"))
            {
                text.transform.LookAt(myCam.transform);
                text.transform.Rotate(new Vector3(0, 180, 0));

            }
        }

        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.tag.Equals("syncCanvas"))
            {
                canvas.transform.LookAt(myCam.transform);
                canvas.transform.Rotate(new Vector3(0, 180, 0));

            }
        }
    }
}