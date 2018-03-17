using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FaceCamera : MonoBehaviour {

    public Camera playerCam;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

            transform.LookAt(playerCam.transform.position);
            transform.Rotate(new Vector3(0, 180, 0));        
    }


    public void CmdMakeTextFaceCamera()
    {
        RpcChangeInClients();
    }


    public void RpcChangeInClients()
    {
        transform.LookAt(playerCam.transform.position);
        transform.Rotate(new Vector3(0, 180, 0));
    }
}
