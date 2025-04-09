using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class OpponentController: MonoBehaviour
{
    [SerializeField] private GameObject gunControllerObject;

    [SerializeField] private GameObject playerObject;
    
    [SerializeField] private GameObject opponentSnowPrefab;

    [SerializeField] private GameObject kdBarObject;

    private PlayerController opponentPlayer;

    private GunController gunController;
    
    private PlayerInfo playerInfo;

    private ShieldController shieldController;

    private BombController bombController;

    private ActionController actionController;

    private KillDeathSection killDeathSection;

    private string tagMqtt = "MQTT";

    private MqttManager mqttManager;

    private int prevAction = 0;

    private int snowStacks = 0;

    private bool isEnabled = false;

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
        killDeathSection = kdBarObject.GetComponent<KillDeathSection>();

        opponentPlayer = playerObject.GetComponent<PlayerController>();
    }

    private void OnMessageArrivedHandler(MqttObj mqttObject)
    {
        if (!isEnabled) return;

        string actionTopic = SettingsController.GetActionTopic();
        string visibilityTopic = SettingsController.GetVisibilityTopic();

        if (mqttObject.ident != GetOpponentNo()) return;

        if (mqttObject.topic == actionTopic)
        {
            prevAction = mqttObject.payload[0];
        }
        else if (mqttObject.topic == visibilityTopic)
        {
            bool isPlayerVisible = mqttObject.payload[0] == 1; // Opponent saw player

            int damageDealt = ProcessAction(prevAction, isPlayerVisible, playerObject.transform.position);
            damageDealt += opponentPlayer.GetSnowDamage();

            if (isPlayerVisible) {
                opponentPlayer.TakeDamage(damageDealt);
            } 
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
            killDeathSection.IncrementKillCount();

            gunController.OnRespawn();
            bombController.OnRespawn();
            shieldController.OnRespawn();
        }
    }

    private int ProcessAction(int action, bool isPlayerVisible, Vector3 playerPos)
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
                damageDealt = bombController.ThrowBomb(isPlayerVisible, playerPos);
                if (isPlayerVisible) {
                    PlaceOpponentSnow();
                }
                break; 
            case 6: // BADMINTON
                damageDealt = actionController.TriggerBadminton(isPlayerVisible, playerPos);
                break; 
            case 7: // GOLF
                damageDealt = actionController.TriggerGolf(isPlayerVisible, playerPos);
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
        if (other.CompareTag("PlayerSnow"))
        {
            TakeDamage(5);
            snowStacks += 1;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerSnow"))
        {
            snowStacks -= 1;
        }
    }

    public int GetSnowDamage()
    {
        return snowStacks * 5;
    }

    private void PlaceOpponentSnow()
    {
        Vector3 snowPos = Camera.main.transform.position;
        snowPos.y = 0;
        Instantiate(opponentSnowPrefab, snowPos, Quaternion.Euler(Vector3.up));
    }

    public void EnableOpponent()
    {
        isEnabled = true;
    }
    
    public void DisableOpponent()
    {
        isEnabled = false;
    }

}