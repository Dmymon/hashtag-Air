using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    public Rigidbody myRigidBody;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float rot = 150.0f;
    public float speed = 3.0f;
    public GameObject interactCircle;
    public GameObject chatCanvas;
    private bool chatActive;
    
    private NetworkIdentity myID;
    [SyncVar]
    public bool interactWithAirToken = false;
    [SyncVar]
    public bool tookAirToken = false;
    private GameObject airToken;

    private ServerOnlyHandler serverObject;

    // Use this for initialization
    void Start()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0);
        myRigidBody = gameObject.GetComponent<Rigidbody>();

        myID = GetComponent<NetworkIdentity>();
        interactCircle.SetActive(false);
        chatActive = false;



    }

    // Update is called once per frame
    void Update()
    {
        if (!myID.isLocalPlayer)
        {
            return;
        }

        ApplyPlayerMovement();

        if (Input.GetKeyDown(KeyCode.C))
        {
            ShowChatOptions();
        }

        if (airToken != null)
        {
            CheckIfTookAirToken();
        }
        else
        {
            DisableAirTokenInteraction();
        }

        //Get the stage object for the server to start the game
        //if (myID.isServer)
      //  {
            if (serverObject == null)
                FindStageObject();
      //  }
    }

    private void FindStageObject()
    {
        serverObject = FindObjectOfType<ServerOnlyHandler>();
       
    }

    public void SetStage()
    {
        if (isServer)
        {
            //RpcSetStageAtPlayers();
            serverObject.SetStage();
            //NetworkServer.Spawn(serverObject.stageObject);
           // serverObject.SetStage();
            //serverObject.SetStage();
        }
    }

    [ClientRpc]
    private void RpcSetStageAtPlayers()
    {
        serverObject.SetStage();
    }
    private void DisableAirTokenInteraction()
    {
        CmdIfByAirToken(false);
        CmdSetTookAirToken(false);

    }

    private void CheckIfTookAirToken()
    {
        if (interactWithAirToken)
        {
            MechanicArm myArm = GetComponent<MechanicArm>();
            if (myArm.isArmOn && !myArm.isArmRotateDown)
            {
                CmdSetTookAirToken(true);
            }
        }
    }

    [Command]
    public void CmdIfByAirToken(bool isNearAirToken)
    {
        interactWithAirToken = isNearAirToken;

    }
    [Command]
    public void CmdSetTookAirToken(bool wasTaken)
    {
        tookAirToken = wasTaken;
        RpcDisableInteraction(wasTaken);

    }
    [ClientRpc]
    public void RpcDisableInteraction(bool wasTaken)
    {
        if (wasTaken)
            interactCircle.SetActive(false);
    }

    private void ShowChatOptions()
    {
        chatActive = !chatActive;
        if (chatActive)
        {
            chatCanvas.SetActive(true);
            chatCanvas.transform.LookAt(transform);
        }
        else
            chatCanvas.SetActive(false);
    }

    private void ApplyPlayerMovement()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * rot;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
       // float angle = Vector3.Angle(transform.localPosition, transform.localRotation.eulerAngles);
        float angle = Vector3.Angle(transform.up, transform.right);

        transform.Rotate(0, 0, -x);
        
        myRigidBody.AddForce(transform.up * angle * z);





    }


    [Command]
    void CmdFire()
    {
        //Create the bullet from the prefab
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        //Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6.0f;

        //Spawn the bullet on the clients
        NetworkServer.Spawn(bullet);

        //Destroy the bullet after 2 seconds
        Destroy(bullet, 2);
    }
    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject otherObject = collision.gameObject;

        switch (otherObject.tag)
        {
            case "player":
                {
                    interactCircle.SetActive(true);
                    break;
                }
            case "airToken":
                {
                    interactCircle.SetActive(true);
                    if (myID.isLocalPlayer)
                    {
                        airToken = otherObject;
                        CmdIfByAirToken(true);
                    }
                    break;
                }
            case "socket":
                {
                    interactCircle.SetActive(true);
                    break;
                }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (interactCircle.activeInHierarchy)
            interactCircle.SetActive(false);

        if (myID.isLocalPlayer)
        {
            if (collision.gameObject.tag == "airToken")
                CmdIfByAirToken(false);
        }
    }

    /*************************************************************/
    
    /*****************************************************************/
}
