using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GasTankHandler : NetworkBehaviour {

    public  float toxicGasAccumulationRate;
    public float gasReleaseUnit;
    private float gasAccumulationTimer;
    public float gasAccumulationTime;
    public RectTransform toxicGasBar;
    public float maxAccumulation = 100;
    public ToxicGasEmitter myGasEmitter;
    private float gasReleaseTime;

    [SyncVar]
    public bool canReleaseGas = true;


    [SyncVar (hook = "OnGasAccumulation")]
    public float gasAccumulation;

    void OnGasAccumulation(float gasIn)
    {
        toxicGasBar.sizeDelta = new Vector2(gasIn * 3, toxicGasBar.sizeDelta.y);

    }
    // Use this for initialization
    void Start () {
        gasAccumulation = 0;
        gasAccumulationTimer = 0;
        gasReleaseTime = 0;

    }
	
	// Update is called once per frame
	void Update () {

        if (isServer)
        {
            CheckGasAccumulation();

        }

        if (isLocalPlayer)
        {
            CheckForGasRelease();
            UpdateGasReleaseTimer();
        }
	}

    private void UpdateGasReleaseTimer()
    {
        if (!isLocalPlayer)
            return;
        if (canReleaseGas)
            return;

        if (gasReleaseTime >= 9)
        {
            gasReleaseTime = 0;
            CmdSetGasReleaseBool(true);
            
        }
        gasReleaseTime += Time.deltaTime;
    }

    [Command]
    public void CmdSetGasReleaseBool(bool canRelease)
    {
        canReleaseGas = canRelease;
    }

    private void CheckForGasRelease()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (canReleaseGas)
            {
                if (gasAccumulation >= gasReleaseUnit)
                    gasAccumulation -= gasReleaseUnit;

                CmdUpdateGasOnClients(gasAccumulation);
                CmdSetGasReleaseBool(false);
            }
        }
    }
    [Command]
    public void CmdUpdateGasOnClients(float gasAmount)
    {
        gasAccumulation = gasAmount;
    }

    //*************************************
    //What happens when Max accumulation reached??!?!
    //**************************************
    private void CheckGasAccumulation()
    {
        if (gasAccumulation >= maxAccumulation)
        {
            EmptyTank();
            return;
        }
        if (gasAccumulationTimer > gasAccumulationTime)
        {
            gasAccumulation += toxicGasAccumulationRate;
            gasAccumulationTimer = 0;
        }
        gasAccumulationTimer += Time.deltaTime;
    }

    private void EmptyTank()
    {
        //CmdUpdateGasOnClients(0);
        gasAccumulation = 0;
        myGasEmitter.EmmitToxicGasByServer();
        //.CmdEmmitToxicGas();
    }
}
