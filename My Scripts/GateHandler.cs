using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GateHandler : NetworkBehaviour {

    public GateOpener myGateOpener;
    public GateSocketHandler[] gateSockets;
    public GameObject gateSocket;

    public float maxVolt;
    public List<GameObject> playersOnGate;
    [SyncVar]
    private int players2Open;
    [SyncVar]
    public bool gateOpen;

    public float gateClosingDelay;
    private float closingTimer;

    public string gateOpenString = "Open";
    public string gateClosedString = "Closed";

	// Use this for initialization
	void Start () {

        gateOpen = false;
        closingTimer = 0;
        playersOnGate = new List<GameObject>();

        if (isServer)
        {
            System.Random r = new System.Random();
            if (NetworkServer.connections.Count > 1)
                players2Open = r.Next(1,NetworkServer.connections.Count);
            else
                players2Open = r.Next(2, 5);

            //  RpcSetPlayer2Open(players2Open);


            gateSockets = new GateSocketHandler[players2Open];
            InitializeGateSockets();
            // gateSockets = FindObjectsOfType<GateSocketHandler>();
        }
    }

    [ClientRpc]
    private void RpcSetPlayer2Open(int players)
    {
        players2Open = players;
    }

    private void InitializeGateSockets()
    {
        int countOtherSide = 1;
        for (int i = 0; i < players2Open; i++)
        {
            Vector3 newSocketPos;

            if (i >= players2Open / 2)
            {
                newSocketPos = new Vector3(transform.position.x - 1, 0, gameObject.transform.position.z + gameObject.transform.localScale.z + 23  + countOtherSide*(gateSocket.transform.localScale.z + 0.5f));
                countOtherSide++;
            }
            else
            {
                if (i > 0)
                {
                    newSocketPos = new Vector3(transform.position.x - 1, 0, gateSockets[i - 1].gameObject.transform.position.z - gateSocket.transform.localScale.z - 0.5f);
                }
                else
                    newSocketPos = new Vector3(transform.position.x - 1, 0, gameObject.transform.position.z - gameObject.transform.localScale.z/2 +4);

            }
            GameObject newSocket = Instantiate<GameObject>(gateSocket, newSocketPos, transform.rotation);
            gateSockets[i] = newSocket.GetComponent<GateSocketHandler>();
            // if (isLocalPlayer)
            //    NetworkServer.Spawn(gateSockets[i].gameObject);
            RpcInstantiateSocket(newSocketPos);

        }

    }
    [ClientRpc]
    private void RpcInstantiateSocket(Vector3 SocketPos)
    {
        GameObject newSocket = Instantiate<GameObject>(gateSocket, SocketPos, transform.rotation);

    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        OpenCloseGate();
    }



    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "player")
        {
            if (!playersOnGate.Contains(collision.gameObject))
                playersOnGate.Add(collision.gameObject);
        }

    }

    private void OpenCloseGate()
    {
        int activeSockets = 0;
        foreach (GateSocketHandler socket in gateSockets)
        {
            if (socket.connected2Player)
                activeSockets++;
        }
        if (activeSockets == players2Open)
        {
            if (!gateOpen)
            {
                gateOpen = true;
                RpcAnimateGate(gateOpen);

            }
        }
        else
        {
            if (gateOpen)
            {
                CheckClosingTimer();
            }
        }
    }

    [ClientRpc]
    private void RpcAnimateGate(bool gateOpen)
    {
        myGateOpener.OpenClose();
    }

    private void CheckClosingTimer()
    {
        if (closingTimer >= gateClosingDelay)
        {
            gateOpen = false;
            RpcAnimateGate(gateOpen);
            closingTimer = 0;
        }

        closingTimer += Time.deltaTime;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "player")
        {
            if (playersOnGate.Contains(collision.gameObject))
                playersOnGate.Remove(collision.gameObject);
        }

    }
}
