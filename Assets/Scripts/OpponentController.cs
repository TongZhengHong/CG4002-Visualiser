using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class OpponentController: MonoBehaviour
{
    [SerializeField] private GameObject gunControllerObject;

    [SerializeField] private GameObject playerObject;
    
    [SerializeField] private GameObject opponentSnowPrefab;

    private PlayerController opponentPlayer;

    private GunController gunController;
    
    private PlayerInfo playerInfo;

    private ShieldController shieldController;

    private BombController bombController;

    private ActionController actionController;

    private string tagMqtt = "MQTT";

    private MqttManager mqttManager;

    private int prevAction = 0;

    private int prevDamage = 0;

    public bool isInSnow = false;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tagMqtt).Length == 0)
        {
            Debug.LogError("Error finding MqttManager from OpponentController.");
            return;
        }
        mqttManager = GameObject.FindGameObjectsWithTag(tagMqtt)[0].gameObject.GetComponent<MqttManager>();
        mqttManager.OnMessageArrived += OnMessageArrivedHandler;

        playerInfo = GetComponent<PlayerInfo>();
        shieldController = GetComponent<ShieldController>();
        bombController = GetComponent<BombController>();
        actionController = GetComponent<ActionController>();
        gunController = gunControllerObject.GetComponent<GunController>();

        opponentPlayer = playerObject.GetComponent<PlayerController>();
    }

    private void OnMessageArrivedHandler(MqttObj mqttObject)
    {
        string actionTopic = SettingsController.GetActionTopic();
        string visibilityTopic = SettingsController.GetVisibilityTopic();

        if (mqttObject.ident != GetOpponentNo()) return;

        if (mqttObject.topic == actionTopic)
        {
            prevDamage = ProcessAction(mqttObject.payload[0]);
            prevAction = mqttObject.payload[0];
        }
        else if (mqttObject.topic == visibilityTopic)
        {
            if (mqttObject.payload[0] == 1) // Opponent saw player 
            {
                if (opponentPlayer.isInSnow) prevDamage += 5;

                opponentPlayer.TakeDamage(prevDamage);
                if (prevAction == 5) { // Check if BOMB action
                    PlaceOpponentSnow();
                }
            } 
            
            prevDamage = 0;
            prevAction = 0;
        }
    }

    public void SyncOpponentInfo(int health, int bullets, int bomb, int shieldHealth, int deaths, int shieldCount)
    {
        playerInfo.SyncHealthAndShield(health, shieldHealth);
        shieldController.SyncShield(shieldCount, shieldHealth);
        gunController.SyncBullets(bullets);
        bombController.SyncBomb(bomb);
    }

    public void TakeDamage(int damage)
    {
        if (playerInfo.IsDeadAfterDamage(damage))
        {
            gunController.OnRespawn();
            bombController.OnRespawn();
            shieldController.OnRespawn();
        }
    }

    private int ProcessAction(int action)
    {
        if (action < 0 || action > 9) return 0;
        int damageDealt = 0;

        switch (action)
        {
            case 1: // SHOOT
                damageDealt = gunController.ShootGun();
                break;
            case 2: // SHIELD
                if (shieldController.ActivateShield()) {
                    playerInfo.ActivateShield();
                }
                break;
            case 3: // RELOAD
                gunController.ReloadBullets();
                break;
            case 4: // LOGOUT
                break; 
            case 5: // BOMB
                damageDealt = bombController.ThrowBomb();
                break; 
            case 6: // BADMINTON
                damageDealt = actionController.TriggerBadminton();
                break; 
            case 7: // GOLF
                damageDealt = actionController.TriggerGolf();
                break; 
            case 8: // FENCING
                damageDealt = actionController.TriggerFencing();
                break; 
            case 9: // BOXING
                damageDealt = actionController.TriggerBoxing();
                break;  
            default:
                break; 
        }
        return damageDealt;
    }

    private int GetOpponentNo()
    {
        int playerNo = SettingsController.GetPlayerNo();
        return playerNo == 1 ? 2 : 1; 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerSnow" && !isInSnow)
        {
            TakeDamage(5);
            isInSnow = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerSnow")
        {
            isInSnow = false;
        }
    }

    private void PlaceOpponentSnow()
    {
        Vector3 snowPos = Camera.main.transform.position;
        snowPos.y = 0;

        GameObject snowObject = Instantiate(opponentSnowPrefab, snowPos, Quaternion.Euler(Vector3.up));
        ParticleSystem snowParticles = snowObject.GetComponentInChildren<ParticleSystem>();
        snowParticles.Play();
    }

}