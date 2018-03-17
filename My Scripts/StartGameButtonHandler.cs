using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class StartGameButtonHandler : MonoBehaviour {

    public ClockHandler myClockHandler;
    public Text myClockText;
    public NetworkIdentity myId;
	// Use this for initialization
	void Start () {

        if (!myId.isServer)
            gameObject.SetActive(false);
        else
        {
            myClockHandler.enabled = false;
            if (!myId.isLocalPlayer && myId.gameObject.tag == "player")
                gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update () {


    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        myClockText.enabled = true;
        myClockHandler.enabled = true;

    }
}
