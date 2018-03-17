using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ArmBatteryHandler : NetworkBehaviour
{

    //public float usageVoltageUnit;
    public RectTransform batteryVoltageBar;
    public float maxVoltage = 100;
    public float basicVoltUnit;
    public NetworkIdentity myId;
    public MechanicArm myMechanicArm;

    public bool batteryEmpty;


    [SyncVar(hook = "OnChangeVoltage")]
    public float actualVoltage;

    void OnChangeVoltage(float totalVoltage)
    {
        batteryVoltageBar.sizeDelta = new Vector2(totalVoltage * 3, batteryVoltageBar.sizeDelta.y);
    }
    // Use this for initialization
    void Start()
    {
        actualVoltage = maxVoltage;
        batteryEmpty = false;
    }

    // Update is called once per frame
    void Update()
    {
         if (!myId.isLocalPlayer)
             return;

        UpdateBattery();
        CheckBatteryStatus();

    }

    private void UpdateBattery()
    {
        if (!myMechanicArm.isArmOn)
        {
            AddBasicVoltUnit(basicVoltUnit);
        }
        else
            AddBasicVoltUnit(-basicVoltUnit);
    }

    private void CheckBatteryStatus()
    {
        if (!batteryEmpty)
        {
            if (actualVoltage <= 0)
                batteryEmpty = !batteryEmpty;
        }
        else
        {
            if (actualVoltage >= basicVoltUnit)
                batteryEmpty = !batteryEmpty;
        }
    }

    public void AddBasicVoltUnit(float voltUnit)
    {

        if (actualVoltage + voltUnit < maxVoltage)
            actualVoltage += voltUnit;
        else if (actualVoltage < 0)
            actualVoltage = 0;
        else
            actualVoltage = maxVoltage;

        CmdSyncAllClients(actualVoltage);
    }

    //
    // Is There Use For This!??
    //
  /*  public void UpdateVoltage()
    {
        if (!myId.isLocalPlayer)
            return;

            actualVoltage -= usageVoltageUnit;

        if (actualVoltage > maxVoltage)
        {
            actualVoltage = maxVoltage;
        }
        else if (actualVoltage < 0)
        {
            actualVoltage = 0;
        }

        CmdSyncAllClients(actualVoltage);
    }
    */
    [Command]
    public void CmdSyncAllClients(float voltage)
    {
            actualVoltage = voltage;

    }
}
