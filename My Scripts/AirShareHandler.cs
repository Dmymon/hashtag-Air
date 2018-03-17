using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class AirShareHandler : NetworkBehaviour {

    public Canvas airSharCanvas;
    public Button startShare;
    public Button stopShare;
    private DraggingHandler myDraggingHandler;
    private GameObject otherPlayer;

    [SyncVar]
    public bool awaitAir = false;
    [SyncVar]
    public bool sharingAir = false;
    [SyncVar]
    public bool gettingAir = false;

    public float airShareUnit;
    private ClockHandler mainClock;

    // Use this for initialization
    void Start () {

        if (isLocalPlayer)
        {
            airSharCanvas.gameObject.SetActive(true);
            airSharCanvas.gameObject.transform.LookAt(transform);
        }
        myDraggingHandler = gameObject.GetComponent<DraggingHandler>();
        
	}
	
	// Update is called once per frame
	void Update () {

        if (isLocalPlayer)
        {

            otherPlayer = myDraggingHandler.otherPlayer;

            if (otherPlayer != null)
            {
                CheckWhileDragging();
                ChackWhileDragged();

            }
            else
            {
                airSharCanvas.gameObject.SetActive(false);
                CmdSetSharingAir(false);
                CmdSetGettingAir(false);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (gettingAir)
                {
                    CmdSetGettingAir(false);
                }
                CmdSetAwaitAir(false);

            }
        }
        //find main clock to update game report
        SearchMainClock();
    }

    private void ChackWhileDragged()
    {
        if (myDraggingHandler.isDragged)
        {
            if (awaitAir)
            {
                CheckCanGetAir();
            }
            if (gettingAir)
            {
                //When other player press stop sharing button
                if (!otherPlayer.GetComponent<AirShareHandler>().sharingAir)
                {
                    CmdSetGettingAir(false);
                }
            }
        }
        //When other player release arm
        if (!myDraggingHandler.isDragged)
        {
            CmdSetGettingAir(false);
        }
    }

    private void CheckWhileDragging()
    {
        if (myDraggingHandler.isDragging)
        {
            if (sharingAir)
            {
                if (!otherPlayer.GetComponent<DraggingHandler>().isDragged)
                {
                    CmdSetSharingAir(false);
                }
            }
            else
                CheckCanShareAir();
        }
        if (!myDraggingHandler.isDragging)
        {
            CmdSetSharingAir(false);
            airSharCanvas.gameObject.SetActive(false);
            startShare.gameObject.SetActive(false);
            stopShare.gameObject.SetActive(false);

        }
    }

    private void CheckCanShareAir()
    {

        if (otherPlayer.GetComponent<AirShareHandler>().awaitAir)
        {
            airSharCanvas.gameObject.SetActive(true);
            if (!startShare.gameObject.activeSelf)
                startShare.gameObject.SetActive(true);
        }
        else
        {
            CmdSetSharingAir(false);
        }
    }
    private void CheckCanGetAir()
    {
        AirShareHandler otherShareHandler = otherPlayer.GetComponent<AirShareHandler>();
        if (otherShareHandler.sharingAir)
        {
            CmdSetGettingAir(true);
            CmdSetAwaitAir(false);
        }
    }

    public void SetAwaitAir(bool waiting)
    {
        CmdSetAwaitAir(waiting);
    }
    [Command]
    public void CmdSetAwaitAir(bool waiting)
    {
        awaitAir = waiting;
        if (waiting)
            UpdatePlayerReport();
    }
    public void StartSharingAir()
    {
        stopShare.gameObject.SetActive(true);
        CmdSetSharingAir(true);
    }
    public void StopSharingAir()
    {
        stopShare.gameObject.SetActive(false);
        CmdSetSharingAir(false);

    }

    [Command]
    public void CmdSetSharingAir(bool sharing)
    { sharingAir = sharing; }
    [Command]
    public void CmdSetGettingAir(bool getting)
    { gettingAir = getting; }

    private void UpdatePlayerReport()
    {
        if (!isServer)
            return;

        string time = mainClock.clockText.text;

        DataCollection dataCollector = FindObjectOfType<DataCollection>();
        dataCollector.AddAirRequest(gameObject.name, time);
    }

    private void SearchMainClock()
    {
        if (!mainClock)
        {
            ClockHandler[] clocks = FindObjectsOfType<ClockHandler>();
            foreach (ClockHandler clock in clocks)
            {
                if (clock.isMasterClock)
                {
                    mainClock = clock;
                    break;
                }
            }
        }
    }
}
