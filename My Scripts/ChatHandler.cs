using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ChatHandler : NetworkBehaviour {

    public Text chatMessage;
    public Text releaseMessageNotifier;
    public Image emojiMessage;
    public Image selectedEmoji;
    public GameObject emojiMenu;

    public Image happyEmoji;
    public Image sadEmoji;
    public Image angryEmoji;
    public Image indifferentEmoji;

    public float timeLimitForMessage2Show;
    float elapsed;

    [SyncVar]
    public string messageChosen = "";

    private bool messageIsShowin;
    public Transform playerCamera;

    ClockHandler mainClock;
    [Command]
    public void CmdSendToServer(string message)
    {
        messageChosen = message;
        RpcSendToClients(message);
    }

    [ClientRpc]
    public void RpcSendToClients(string message)
    {
        chatMessage.text = message;
        messageIsShowin = true;

        if (message.Length > 0)
            releaseMessageNotifier.enabled = true;
        else
            releaseMessageNotifier.enabled = false;

    }
    // Use this for initialization
    void Start() {
        elapsed = 0;
        chatMessage.text = "";
        messageIsShowin = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {

            if (Input.GetKeyDown(KeyCode.R))
                HideMessage();
        }
        //find main clock for game report update
        SearchMainClock();

        if (!isLocalPlayer)
            releaseMessageNotifier.enabled = false;

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
    private void HideMessage()
    {
        CmdSendToServer("");
        //    if (emojiMessage.enabled)
        //       emojiMessage.enabled = false;
        CmdHideEmoji();

    }

    [Command]
    private void CmdHideEmoji()
    {
        RpcHideEmoji();
    }

    [ClientRpc]
    private void RpcHideEmoji()
    {
        if (emojiMessage.enabled)
            emojiMessage.enabled = false;

        releaseMessageNotifier.enabled = false;
    }

    private void CheckMessageTime()
    {
        if (messageIsShowin)
        {
            if (elapsed >= timeLimitForMessage2Show)
            {
                elapsed = 0;
                CmdSendToServer("");
            }
            else
                elapsed += Time.deltaTime;
        }
        else
            elapsed = 0;

    }

    public void ShowMessage(string message)
    {
        HideMessage();
        CmdSendToServer(message);

    }

    public void ShowEmojiMenu()
    {
        if (emojiMenu.activeSelf)
            emojiMenu.SetActive(false);
        else
            emojiMenu.SetActive(true);
    }

    public void ShowEmoji(string type)
    {
        string time = mainClock.clockText.text;
        if (!isLocalPlayer)
            return;

        HideMessage();
        CmdShowEmojiToPlayers(time, type);
    }

    [Command]
    private void CmdShowEmojiToPlayers(string time, string type)
    {
        DataCollection dataCollector = FindObjectOfType<DataCollection>();
        dataCollector.AddEmojiUse(name, time, type);
        RpcShowEmojiToPlayers(type);
    }

    [ClientRpc]
    private void RpcShowEmojiToPlayers(string type)
    {
        switch (type)
        {
            case "happy":
                {
                    selectedEmoji = happyEmoji;
                    break;
                }
            case "sad":
                {
                    selectedEmoji = sadEmoji;
                    break;
                }
            case "angry":
                {
                    selectedEmoji = angryEmoji;
                    break;
                }
            case "indifferent":
                {
                    selectedEmoji = indifferentEmoji;
                    break;
                }
            default: { selectedEmoji = null; break; }
        }

        emojiMessage.sprite = selectedEmoji.sprite;
        emojiMessage.enabled = true;
        releaseMessageNotifier.enabled = true;
    }

    public override void OnStartClient()
    {

    }
}
