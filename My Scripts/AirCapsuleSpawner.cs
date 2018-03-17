using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class AirCapsuleSpawner : NetworkBehaviour
{

    public NetworkStartPosition[] spawnPoints;

    public GameObject[] airCapsulesOnGame;
    public GameObject airCapsule;

    [SyncVar]
    public int ammountToSpawn;

    private float currentMinute;
    public int maxUnits = 3;

    float currentTimeInMinuts;
    float checkingTime;

    // Use this for initialization
    void Start()
    {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();

        // for (int i = 0; i < 3; i++)

        currentTimeInMinuts = 0;
        checkingTime = 0.5f;
        currentMinute = -0.5f;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        if (CheckTime2Spawn())
            HandleSpawning();
        
    }

private bool CheckTime2Spawn()
{
    currentTimeInMinuts += Time.deltaTime / 60;
    if (currentTimeInMinuts > currentMinute + checkingTime)
    {
        currentMinute += checkingTime;
        return true;
    }
        return false;

}
    private void HandleSpawning()
    {
        
        switch (currentMinute.ToString())
        {
            case "0":
                {
                    ammountToSpawn = 3;
                    break;
                }
            case "0.5":
                {
                    ammountToSpawn = CheckForActiveCapsules();
                    break;
                }
            case "1":
                {
                    CmdDestroyAirCapsules();
                    ammountToSpawn = 2;
                    break;
                }
            case "1.5":
                {
                    ammountToSpawn = CheckForActiveCapsules();
                    break;
                }
            case "2":
                {
                    CmdDestroyAirCapsules();
                    ammountToSpawn = 1;
                    break;
                }
            case "2.5":
                {
                    ammountToSpawn = CheckForActiveCapsules();
                    break;
                }
            case "3":
                {
                    CmdDestroyAirCapsules();
                    ammountToSpawn = 1;
                    break;
                }
            default: { currentMinute = 2.5f; currentTimeInMinuts = 2; break; }
        }

        if (ammountToSpawn > 0)
        {
            InstantiateCapsulesInArray();
        }

    }

    private int CheckForActiveCapsules()
    {
        int activeCapsules = 0;

        foreach (GameObject g in airCapsulesOnGame)
        {
            if (g != null && g.activeInHierarchy)
                activeCapsules += 1;
        }
        if (activeCapsules == 0)
        {
            if (ammountToSpawn > 1)
                return ammountToSpawn - 1;
            else
                return 1;
        }
        else
            return 0;
    }

   
    public void CmdSetSpawnPointsForCapsules()
    {
        airCapsulesOnGame = new GameObject[ammountToSpawn];

        float farestPlayer = -140;
        Vector3 farest = new Vector3(0,0,0);
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            if (player.gameObject.transform.position.x > farestPlayer)
            {
                farest = player.transform.position;
                farestPlayer = player.gameObject.transform.position.x;
            }
        }
        for (int i = 0; i < ammountToSpawn; i++)
        {
            GameObject toSpawnNow = Instantiate(airCapsule, airCapsule.transform.position, airCapsule.transform.rotation);
            Vector3 spawnPoint =  new Vector3(UnityEngine.Random.Range(farest.x-8, farest.x+8), farest.y, UnityEngine.Random.Range(farest.z-8.0f, farest.z+8.0f)); 
            toSpawnNow.transform.position = spawnPoint;
            airCapsulesOnGame[i] = toSpawnNow;

            NetworkServer.Spawn(toSpawnNow);
        }

    }


    [ClientRpc]
    public void RpcSetSpawnPointsForCapsules(Vector3[] airSpawns)
    {

    }
    
    public void CmdDestroyAirCapsules()
    {
        foreach (GameObject g in airCapsulesOnGame)
            NetworkServer.Destroy(g);

       // RpcDestroyAirCapsules();
    }
    [ClientRpc]
    public void RpcDestroyAirCapsules()
    {
        foreach (GameObject g in airCapsulesOnGame)
            NetworkServer.Destroy(g);
    }


    public void InstantiateCapsulesInArray()
    {
        CmdSetSpawnPointsForCapsules();
    }


}
