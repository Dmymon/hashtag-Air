using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ClockHandler : NetworkBehaviour {

    public NetworkIdentity playerId;

    public Text clockText;
    public Button fasterBtn;
    public Button slowerBtn;

    [SyncVar]
    public int seconds;
    [SyncVar]
    public int minuts;

    [SyncVar]
    public string minutsInText = " ";
    [SyncVar]
    public string secondsInText = " ";

    private float actualTime;

    [SyncVar]
    public float clockRate = 0;

    public ClockHandler clockHandler;
    [SyncVar]
    public bool isMasterClock = false;
    private float fixedTimeUnit;

    // Use this for initialization
    public void Start () {

        if (isServer)
        {
            if (clockHandler == null)
            {
                clockHandler = this;
                isMasterClock = true;
                actualTime = 0;
                seconds = 0;
                minuts = 20;
                fixedTimeUnit = 0.1f;
            }
            if (isLocalPlayer)
            {
                //isMasterClock = true;
                fasterBtn.gameObject.SetActive(true);
                slowerBtn.gameObject.SetActive(true);
            }
            if (gameObject.tag == "player" && !isLocalPlayer)
                isMasterClock = false;


        }
        else 
        {
            SetClockReference();
        }
         if (!playerId.isLocalPlayer)
        {
            if (gameObject.tag == "player")
                clockText.gameObject.SetActive(false);
        }
    }

    private void SetClockReference()
    {
        ClockHandler[] timers = FindObjectsOfType<ClockHandler>();

        for (int i = 0; i < timers.Length; i++)
        {
            if (timers[i].isMasterClock)
            {
                if (timers[i].gameObject.tag == "player")
                    clockHandler = timers[i];
            }
        }
        if (clockHandler == null)
        {
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i].isMasterClock)
                {
                    clockHandler = timers[i];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isServer)
            CheckClock();

        if (clockHandler)
        {
            clockText.text = clockHandler.minutsInText + " : " + clockHandler.secondsInText;
        }
        else
        {
            SetClockReference();
        }

        if (!playerId.isLocalPlayer)
        {
            if (gameObject.tag == "player")
                clockText.gameObject.SetActive(false);
        }
    }

    private void CheckClock()
    {
        if (actualTime > clockRate)
        {
            if (minuts == 0 && seconds == 0)
            {
                //GAME OVER!!!!
                clockRate = 0;
                //RpcGameOver();
            }
            else if (seconds == 0)
            {
                seconds = 59;
                minuts-=1;
            }
            else
            {
                seconds-=1;
            }


            actualTime = 0;
            UpdateClock();
        }
        actualTime += Time.deltaTime;
    }

    [ClientRpc]
    private void RpcGameOver()
    {
        if (gameObject.tag == "player")
        {
            if (isServer)
            {
                isMasterClock = false;
                clockHandler = null;
            }
            gameObject.SetActive(false);

        }
    }

    private void UpdateClock()
    {
        if (minuts < 10)
            minutsInText = "0" + minuts;
        else
            minutsInText = minuts.ToString();

        if (seconds < 10)
            secondsInText = "0" + seconds;
        else
            secondsInText = seconds.ToString();

        RpcSyncClock();
    }

    [ClientRpc]
    public void RpcSyncClock()
    {
        if (clockHandler)
        {
            clockText.text = clockHandler.minutsInText + " : " + clockHandler.secondsInText;
        }
    }

    public void SpeedClockRate()
    {
        if (isLocalPlayer)
            CmdUpdateClockRate(-fixedTimeUnit);
        else
            clockRate += -fixedTimeUnit;
    }
    public void SlowDownClock()
    {
        if (isLocalPlayer)
            CmdUpdateClockRate(fixedTimeUnit);
        else
            clockRate += fixedTimeUnit;
    }

    [Command]
    public void CmdUpdateClockRate(float timeUnit)
    {
        clockRate += timeUnit;
    }

}
