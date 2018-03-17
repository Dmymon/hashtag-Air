using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ToxicGasEmitter : NetworkBehaviour {

    public NetworkIdentity myId;
    public ParticleSystem toxicGasRelease;
    
	// Use this for initialization
	void Start () {

	}

    // Update is called once per frame
    void Update()
    {

        if (!myId.isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!toxicGasRelease.isPlaying)
            {
                CmdEmmitToxicGas();
            }
           
        }
    }

  [Command]
   public void CmdEmmitToxicGas()
   {
        toxicGasRelease.Play();
        RpcEmmitToxicGas();
    }

    [ClientRpc]
    private void RpcEmmitToxicGas()
    {
        toxicGasRelease.Play();

    }
    public void EmmitToxicGasByServer()
    {
        if (!isServer)
            return;

        toxicGasRelease.Play();
        RpcEmmitToxicGas();
    }
}
