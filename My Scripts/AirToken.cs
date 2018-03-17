using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;


public class AirToken : NetworkBehaviour {

    public GameObject player;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        if (player != null)
        {
            if (player.GetComponent<PlayerController>().tookAirToken)
            {
                NetworkServer.Destroy(gameObject);
                Destroy(gameObject);                
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag.Equals("player"))
        {
            player = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("player"))
        {
            player = null;
        }
    }

}
