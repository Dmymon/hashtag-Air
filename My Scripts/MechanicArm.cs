using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Networking;
using System;

public class MechanicArm : NetworkBehaviour
{

    public float angle;
    public float maxAngle;
    public GameObject mechanicArm;
    public ArmBatteryHandler armBattery;
    public GameObject hingePoint;

    public float armUseDelay;
    public float armUseTimer;

    [SyncVar]
    public bool isArmOn;
    [SyncVar]
    public bool isArmRotateDown;
    [SyncVar]
    public bool isDragging;
    [SyncVar]
    public bool canMoveArm;

    private NetworkIdentity myID;
    private float startRotX;



    // Use this for initialization
    void Start()
    {
        startRotX = mechanicArm.transform.rotation.eulerAngles.x;

        isArmRotateDown = false;
        isDragging = false;
        isArmOn = false;
        canMoveArm = true;

        myID = gameObject.GetComponentInParent<NetworkIdentity>();

        armUseDelay = 3;
        armUseTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {

        if (!myID.isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isArmOn)
            {
                if (canMoveArm && !armBattery.batteryEmpty)
                {
                    CmdActivateArm(true, true);
                    CmdUpdatePlayerReport();
                }
            }
            if (isDragging)
            {
                CmdSetDraggingStatus(!isDragging);
            }
        }

        CheckIfCanMoveArm();

        
    }

    [Command]
    public void CmdUpdatePlayerReport()
    {
        UpdatePlayerReport();
    }

    private void CheckIfCanMoveArm()
    {
        if (isArmOn)
        {
            CmdMoveArm();

            if (canMoveArm)
                CmdSetCanMoveArm(false);

            if (armBattery.batteryEmpty)
            {
                if (isDragging)
                CmdSetDraggingStatus(false);
            }
        }
        else
        {
            CheckUseDelay();
        }
    }

    private void CheckUseDelay()
    {
        if (armUseTimer >= armUseDelay)
        {
            CmdSetCanMoveArm(true);
            armUseTimer = 0;
        }
        armUseTimer += Time.deltaTime;
    }

    [Command]
    public void CmdSetCanMoveArm(bool newState)
    {
        canMoveArm = newState;
    }
    [Command]
    public void CmdActivateArm(bool newIsOn,bool newDown)
    {
        isArmOn = newIsOn;
        isArmRotateDown = newDown;
    }
    [Command]
    public void CmdMoveArm()
    {

        RpcShowArmMove();
        //isOn = false;

    }

    [ClientRpc]
    public void RpcShowArmMove()
    {

        if (isArmRotateDown)
        {
            if (mechanicArm.transform.rotation.eulerAngles.x < startRotX + maxAngle)
                mechanicArm.transform.Rotate(angle, 0, 0);
            else
            {
                if (myID.isLocalPlayer)
                    CmdActivateArm(isArmOn, false);
            }
        }

        else if (mechanicArm.transform.rotation.eulerAngles.x > startRotX + angle)
        {
            if (!isDragging)
                mechanicArm.transform.Rotate(-angle, 0, 0);
        }
        else
        {
            mechanicArm.transform.rotation.eulerAngles.Set(0, 0, 0);

            if (myID.isLocalPlayer)
            {
                CmdActivateArm(false, isArmRotateDown);
            }

        }

    }

    private void UpdatePlayerReport()
    {
        if (!isServer)
            return;

        DataCollection dataCollector = FindObjectOfType<DataCollection>();
        dataCollector.AddArmUseCount(gameObject.name);
    }

    [Command]
    public void CmdSetDraggingStatus(bool newStat)
    {
        isDragging = newStat;

        if (newStat)
            UpdatePlayerReport();
    }
}

