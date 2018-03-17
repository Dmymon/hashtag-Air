using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class EnterNameCanvasHandler : NetworkBehaviour {

    public Canvas enterNameCanvas;
    public InputField inputField;
    public Text NameTextUi;
    public GameObject player;
    public NetworkIdentity myNetId;

    [SyncVar]
    public string playerName = string.Empty;
	// Use this for initialization
	void Start () {

        if (!myNetId.isLocalPlayer)
            enterNameCanvas.gameObject.SetActive(false);
        else
            enterNameCanvas.gameObject.SetActive(true);


    }

    // Update is called once per frame
    void Update () {

        if (!myNetId.isLocalPlayer)
            return;

        if (enterNameCanvas.isActiveAndEnabled)
        {
            if (!inputField.isFocused)
                inputField.ActivateInputField();
        }


	}

    public void OnEndEdit()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string name = inputField.text;
            CmdSetPlayersName(name);
        }

    }
    [Command]
    public void CmdSetPlayersName(string newName)
    {
        playerName = newName;
        RpcShowPlayerName(newName);

        UpdatePlayerReport();
    }
    private void UpdatePlayerReport()
    {
        DataCollection dataCollector = FindObjectOfType<DataCollection>();
        dataCollector.AddPlayerName(playerName);
    }
    [ClientRpc]
    public void RpcShowPlayerName(string newName)
    {
        player.name = newName;
        NameTextUi.text = newName;
        enterNameCanvas.gameObject.SetActive(false);
        NameTextUi.gameObject.SetActive(true);
    }
    public override void OnStartLocalPlayer()
    {
        
    }
}
