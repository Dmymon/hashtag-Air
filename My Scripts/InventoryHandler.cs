using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class InventoryHandler : NetworkBehaviour {

    public Text airSlotText;
    public Canvas InventoryCanvas;
    [SyncVar(hook = "OnChangeAirCapsules")]
    public int airCapsulesInInventory;

    public NetworkIdentity myNetId;

    private void Start()
    {
        if (myNetId.isLocalPlayer)
            InventoryCanvas.enabled = true;
        else
            InventoryCanvas.enabled = false;

    }


    private void OnChangeAirCapsules(int newAmount)
    {
        airSlotText.text = "X " + newAmount;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!myNetId.isLocalPlayer)
            return;

        if (collision.gameObject.tag == "airToken")
        {
            CmdChangeCapsulesAmount();
        }

    }

    [Command]
    public void CmdChangeCapsulesAmount()
    {
        airCapsulesInInventory += 1;

    }


    public override void OnStartLocalPlayer()
    {
        airCapsulesInInventory = 0;
    }
}
