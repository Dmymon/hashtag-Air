using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DraggingHandler : NetworkBehaviour {

    public NetworkIdentity myID;
    public MechanicArm myArm;
    public GameObject otherPlayer;
    private GameObject otherPlayersHingePoint;

    [SyncVar]
    public bool awaitDragging = false;
    [SyncVar]
    public bool isDragging = false;
    [SyncVar]
    public bool isDragged = false;
    [SyncVar]
    public string otherPlayerName;

    private ClockHandler mainClock;

    void Update()
    {
        if (myID.isLocalPlayer)
        {

            UpdateWhenDragging();
            UpdateWhenDragged();
            if (!isDragging && !isDragged)
            {
                otherPlayer = null;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                HandleRelease();
            }
        }
        //find clock to update time for game report
        SearchMainClock();
    }

    private void HandleRelease()
    {
        if (isDragged)
        {
            CmdGetDragged(false);
        }
        CmdSetAwaitDrag(false);
    }

    private void UpdateWhenDragged()
    {
        if (isDragged)
        {
            if (otherPlayer.GetComponent<DraggingHandler>().isDragging)
                LookAtPlayer();
            else
                CmdGetDragged(false);
        }
    }

    private void UpdateWhenDragging()
    {
        if (isDragging)
        {
            //checks if player disconnected arm with button click
            if (!myArm.isDragging)
                CmdDragOther(false, string.Empty);
            else
            {
                CheckOtherPlayerStillDragged();
            }
        }
    }

    private void CheckOtherPlayerStillDragged()
    {
        if (otherPlayer != null)
        {
            DraggingHandler otherPlayerDragHandler = otherPlayer.GetComponent<DraggingHandler>();
            if (!otherPlayerDragHandler.isDragged && !otherPlayerDragHandler.awaitDragging)
                CmdDragOther(false, string.Empty);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!myID.isLocalPlayer)
            return;

        GameObject otherObject = collision.gameObject;

        switch (otherObject.tag)
        {
            case "player":
                {
                    otherPlayer = otherObject;
                    DraggingHandler otherPlayerDragHandler = otherObject.GetComponent<DraggingHandler>();
                    HandleDragging(otherPlayerDragHandler);
                    break;
                }
            case "socket":
                {
                    GateSocketHandler socket = otherObject.GetComponent<GateSocketHandler>();
                    if (!socket.connected2Player)
                        Connect2Socket();
                    break;
                }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!myID.isLocalPlayer)
            return;

        GameObject otherObject = collision.gameObject;

        switch (otherObject.tag)
        {
            case "socket":
                {
                    if (isDragging)
                        CmdDragOther(false,string.Empty);
                    break;
                }
        }
    }

    private void Connect2Socket()
    {
        if (myArm.isArmOn)
            CmdDragOther(true, string.Empty);

    }

    private void HandleDragging(DraggingHandler otherPlayer)
    {
        if (!myID.isLocalPlayer)
            return;

        if (awaitDragging)
        {
            if (otherPlayer.isDragging)
            {
                CreateJoint(otherPlayer);
                CmdGetDragged(true);
                CmdSetAwaitDrag(false);
            }
        }
        else if (otherPlayer.awaitDragging)
        {
            if (myArm.isArmOn)
            {
                CmdDragOther(true,otherPlayer.name);

            }
        }
    }

    private void LookAtPlayer()
    {
        transform.position = otherPlayersHingePoint.transform.position;
        
    }

    [Command]
    public void CmdDragOther(bool dragging, string otherPlayerName)
    {
        if (!isDragging)
        {
            if (dragging)
            {
                this.otherPlayerName = otherPlayerName;
                UpdatePlayerReport(true);
            }
        }
        isDragging = dragging;
        myArm.isDragging = dragging;

    }

    [Command]
    public void CmdGetDragged(bool dragged)
    {
        isDragged = dragged;
    }

    public void SetAwaitDrag(bool state)
    {
        CmdSetAwaitDrag(state);
    }
    [Command]
    public void CmdSetAwaitDrag(bool newStat)
    {
        awaitDragging = newStat;

        if (newStat)
            UpdatePlayerReport(false);

    }

    private void CreateJoint(DraggingHandler otherPlayer)
    {
        otherPlayersHingePoint = otherPlayer.gameObject.GetComponent<MechanicArm>().hingePoint;
    }

    private void UpdatePlayerReport(bool dragging)
    {
        string time = mainClock.clockText.text;

        DataCollection dataCollector = FindObjectOfType<DataCollection>();
        if (dragging)
        {
            dataCollector.AddDraggingOther(gameObject.name, time, otherPlayerName);
        }
        else
        {
            dataCollector.AddDraggingRequest(gameObject.name, time);
        }
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
