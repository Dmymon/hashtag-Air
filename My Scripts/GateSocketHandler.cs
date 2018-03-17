using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GateSocketHandler : NetworkBehaviour {

    public GameObject connectedPlayer;
    public bool connected2Player = false;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if (connectedPlayer != null)
        {
            if (connectedPlayer.GetComponent<DraggingHandler>().isDragging)
                CmdSetSocketState(true);

        }
        else
            CmdSetSocketState(false);

    }

    public void CmdSetSocketState(bool socketState)
    {
        connected2Player = socketState;
    }

    private void OnCollisionStay(Collision collision)
    {       
        if (collision.gameObject.tag == "player")
        {
            if (connectedPlayer == null)
            {
                connectedPlayer = collision.gameObject;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        connectedPlayer = null;          
        
    }
}
