using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class DamageValuePerObject
{

    public static float bulletDamage = 10;
    public static float gasDamage = 5;
    public static float armUseDamage = 0.5f;
    public static float basicAirLoss = 0.2f;
    public static float airLossWhileDragged = -0.001f;
    public static float airLossWhileDragging = 0.0f;
    public static float airLossWhileSharingAir = 0.03f;
    public static float airLossWhileGettingAir = -0.03f;
    public static float airToken = -0.14f;

}
public class Health : NetworkBehaviour {

    public const int maxHealth = 100;

    [SyncVar (hook = "OnChangeHealth")]
    public float currentHealth = maxHealth;
    [SyncVar]
    public float damage = 0;

    public RectTransform healthbar;
    public bool destroyOnDeath;
    private NetworkStartPosition[] spawnPoints;

    public PlayerController playerController;
    private NetworkIdentity myId;

    private DraggingHandler dragStatus;
    private AirShareHandler airShareStatus;

    public GameObject myGasEmitterForRef;
    public float airLossTime;
    float timer;

    private bool canTakeParticleDamage = false;
    private float particleDamageTimer = 0;
    private const float particleDamageTime = 5.0f;

    public Color goodHealth;
    public Color badHealth;
    private string gasHittingPlayerName;

    private ClockHandler mainClock;

    public void TakeDamage(float amount)
    {
        if(!isServer)
        {
            return;
        }
        currentHealth -= amount;
        if (amount == DamageValuePerObject.gasDamage)
        {
            if (gasHittingPlayerName != null)
                UpdatePlayerReport(false, gasHittingPlayerName);

        }

        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                UpdatePlayerReport(true,string.Empty);
                RpcStopPlayer();
            }

            else
            {
                currentHealth = maxHealth;
                RpcRespawn();
            }
        }
        else if (currentHealth > maxHealth)
            currentHealth = maxHealth;

    }

    private void UpdatePlayerReport(bool gameTime, string hittingPlayer)
    {
        string time = mainClock.clockText.text;

        if (gameTime)
        {
            DataCollection dataCollector = FindObjectOfType<DataCollection>();
            dataCollector.AddGameTime(name, time);
        }
        else
        {
            DataCollection dataCollector = FindObjectOfType<DataCollection>();
            dataCollector.AddGasHit(name, time, hittingPlayer);
        }
    }

    [ClientRpc]
    public void RpcStopPlayer()
    {
        if (isServer)
        {
            foreach (ClockHandler clock in FindObjectsOfType<ClockHandler>())
            {
                if (clock.isServer && clock.gameObject.tag != "player")
                {
                    clock.enabled = true;
                    clock.Start();
                    clock.minuts = mainClock.minuts;
                    clock.seconds = mainClock.seconds;
                }
            }
        }
        DestroyImmediate(gameObject);
    }

    void OnChangeHealth(float health)
    {
        Color color = healthbar.GetComponent<Image>().color;
            if (health < 30)
                color = badHealth;
            else
               color = goodHealth;
        
        healthbar.sizeDelta = new Vector2(health*3 , healthbar.sizeDelta.y);
        healthbar.GetComponent<Image>().color = color;
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (!isLocalPlayer)
            return;

        Vector3 spawnPoint = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
            transform.position = spawnPoint;
        }

    }
    // Use this for initialization
    void Start()
    {

        myId = gameObject.GetComponent<NetworkIdentity>();
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        timer = 0;

        dragStatus = gameObject.GetComponent<DraggingHandler>();
        airShareStatus = gameObject.GetComponent<AirShareHandler>();

    }

    void OnCollisionEnter(Collision collision)
    {
        Health health = GetComponent<Health>();

        if (health != null)
        {
            switch (collision.gameObject.tag)
            {
                case "bullet":
                    {
                        health.TakeDamage(DamageValuePerObject.bulletDamage);
                        break;
                    }
                case "arm":
                    {
                        health.TakeDamage(DamageValuePerObject.armUseDamage);
                        break;
                    }
                case "toxicGas":
                    {
                        health.TakeDamage(DamageValuePerObject.gasDamage);
                        break;
                    }

            }
        }
 
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.Equals(myGasEmitterForRef))
            return;

        if (canTakeParticleDamage)
        {
            TakeDamage(DamageValuePerObject.gasDamage);
            canTakeParticleDamage = false;

            gasHittingPlayerName = other.GetComponentInParent<PlayerController>().gameObject.name;
        }
    }

    private void CheckGasDamageTime()
    {
        particleDamageTimer += Time.deltaTime;
        if (particleDamageTimer > particleDamageTime)
        {
            canTakeParticleDamage = true;
            particleDamageTimer = 0;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (playerController.interactWithAirToken)
            CheckIfTokenCollected();

        CheckDamageByAirSharing();
        CheckDamageByDragStatus();
        FixedAirLoss();
        CheckGasDamageTime();

        //find main clock to send to game report when destroyed
        SearchMainClock();
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

    private void CheckIfTokenCollected()
    {
        if (gameObject.GetComponent<MechanicArm>().isArmOn)
            TakeDamage(DamageValuePerObject.airToken);
    }

    private void CheckDamageByAirSharing()
    {
        if (airShareStatus.sharingAir)
            TakeDamage(DamageValuePerObject.airLossWhileSharingAir);
        else if (airShareStatus.gettingAir)
            TakeDamage(DamageValuePerObject.airLossWhileGettingAir);

    }

    [Command]
    public void CmdSetDamageVar(float newDamage)
    {
        damage = newDamage;
    }

    public void CheckDamageByDragStatus()
    {
        if (dragStatus.isDragged)
            TakeDamage(DamageValuePerObject.airLossWhileDragged);
        else if (dragStatus.isDragging)
            TakeDamage(DamageValuePerObject.airLossWhileDragging);
    }


    private void FixedAirLoss()
    {
        timer += Time.deltaTime;
        if (timer / 60 > airLossTime)
        {
            TakeDamage(DamageValuePerObject.basicAirLoss);
            timer = 0;
        }
    }

}
