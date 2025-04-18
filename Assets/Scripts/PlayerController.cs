using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerController: MonoBehaviour
{
    [SerializeField] private GameObject gunControllerObject;

    [SerializeField] private GameObject opponentPlayerObject;

    [SerializeField] private GameObject playerSnowPrefab;

    [SerializeField] private GameObject kdBarObject;

    private OpponentController opponentPlayer;

    private GunController gunController;
    
    private PlayerInfo playerInfo;

    private ShieldController shieldController;

    private BombController bombController;

    private ActionController actionController;

    private KillDeathSection killDeathSection;

    private string tagMqtt = "MQTT";

    private MqttManager mqttManager;

    private int currentAction = 1;

    private int snowStacks = 0;

    [SerializeField] private TMP_Text actionButtonText;

    [SerializeField] private TMP_Text hitMissText;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tagMqtt).Length == 0)
        {
            Debug.LogError("Error finding MqttManager from PlayerController.");
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

        opponentPlayer = opponentPlayerObject.GetComponent<OpponentController>();
    }

    public void TakeDamage(int damage)
    {
        if (playerInfo.IsDeadAfterDamage(damage))
        {
            killDeathSection.IncrementDeathCount();

            gunController.OnRespawn();
            bombController.OnRespawn();
            shieldController.OnRespawn();
        }
    }

    public void PlayAction()
    {
        int damageDealt = ProcessAction(currentAction, ReticlePointer.isLookingAtOpponent, opponentPlayerObject.transform.position);
        damageDealt += opponentPlayer.GetSnowDamage();
        DealDamageToOpponent(damageDealt);
    }

    public void NextAction()
    {
        if (actionButtonText != null)
        {
            currentAction = currentAction >= 9 ? 1 : currentAction + 1;
            string[] actionStrings = { "None", "Shoot", "Shield", "Reload", "Logout", 
                "Bomb", "Badminton", "Golf", "Fencing", "Boxing" };
            actionButtonText.text = actionStrings[currentAction];
        }
    }

    private void OnMessageArrivedHandler(MqttObj mqttObject)
    {
        int playerNumber = SettingsController.GetPlayerNo();
        string actionTopic = SettingsController.GetActionTopic();

        if (mqttObject.topic != actionTopic) return;
        if (mqttObject.ident != playerNumber) return;

        int action = mqttObject.payload[0];
        int damageDealt = ProcessAction(action, ReticlePointer.isLookingAtOpponent, opponentPlayerObject.transform.position);
        damageDealt += opponentPlayer.GetSnowDamage();
        DealDamageToOpponent(damageDealt);

        string visibilityTopic = SettingsController.GetVisibilityTopic();
        PublishPlayerViz(visibilityTopic, playerNumber);
    }

    public void SyncPlayerInfo(int health, int bullets, int bomb, int shieldHealth, int deaths, int shieldCount)
    {
        playerInfo.SyncHealthAndShield(health, shieldHealth);
        shieldController.SyncShield(shieldCount, shieldHealth);
        gunController.SyncBullets(bullets);
        bombController.SyncBomb(bomb);
        killDeathSection.UpdateDeathCount(deaths);
    }

    private void DealDamageToOpponent(int damageDealt)
    {
        if (ReticlePointer.isLookingAtOpponent && opponentPlayer != null)
        {
            opponentPlayer.TakeDamage(damageDealt);
        } 
    }

    private int ProcessAction(int action, bool isOpponentVisible, Vector3 oppPosition)
    {
        if (action < 0 || action > 9) return 0;
        int damageDealt = 0;

        switch (action)
        {
            case 1: // SHOOT
                damageDealt = gunController.ShootGun();
                StopAllCoroutines();
                StartCoroutine(ShowHitMissText(ReticlePointer.isLookingAtOpponent ? "Hit!" : "Miss..."));
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
                damageDealt = bombController.ThrowBomb(isOpponentVisible, oppPosition);
                if (isOpponentVisible) {
                    PlacePlayerSnow();
                }
                StopAllCoroutines();
                StartCoroutine(ShowHitMissText(isOpponentVisible ? "Hit!" : "Miss..."));
                break; 
            case 6: // BADMINTON
                damageDealt = actionController.TriggerBadminton(isOpponentVisible, oppPosition);
                StopAllCoroutines();
                StartCoroutine(ShowHitMissText(isOpponentVisible ? "Hit!" : "Miss..."));
                break; 
            case 7: // GOLF
                damageDealt = actionController.TriggerGolf(isOpponentVisible, oppPosition);
                StopAllCoroutines();
                StartCoroutine(ShowHitMissText(isOpponentVisible ? "Hit!" : "Miss..."));
                break; 
            case 8: // FENCING
                damageDealt = actionController.TriggerFencing();
                StopAllCoroutines();
                StartCoroutine(ShowHitMissText(isOpponentVisible ? "Hit!" : "Miss..."));
                break; 
            case 9: // BOXING
                damageDealt = actionController.TriggerBoxing();
                StopAllCoroutines();
                StartCoroutine(ShowHitMissText(ReticlePointer.isLookingAtOpponent ? "Hit!" : "Miss..."));
                break;
            default:
                break; 
        }
        return damageDealt;
    }

    private IEnumerator ShowHitMissText(string hitOrMiss)
    {
        if (hitMissText == null) yield break;
        Vector3 originalPosition = hitMissText.gameObject.transform.localPosition;

        hitMissText.text = hitOrMiss;
        hitMissText.gameObject.SetActive(true);

        float elapsed = 0f;
        float duration = 0.5f;
        float magnitude = 2.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            Vector3 newLoc = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            hitMissText.gameObject.transform.localPosition = newLoc;

            elapsed += Time.deltaTime;
            yield return null;
        }

        hitMissText.gameObject.transform.localPosition = originalPosition;
        hitMissText.gameObject.SetActive(false);
    }

    private void PublishPlayerViz(string topic, int playerNo) 
    {
        if (mqttManager != null) 
        {
            Debug.Log("Sending: " + ReticlePointer.isLookingAtOpponent.ToString());
            mqttManager.PublishVisibility(topic, playerNo, ReticlePointer.isLookingAtOpponent); 
        }
    }

    private void PlacePlayerSnow()
    {
        Vector3 snowPos = opponentPlayerObject.transform.position;
        snowPos.y = 0;
        Instantiate(playerSnowPrefab, snowPos, Quaternion.Euler(Vector3.up));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OpponentSnow"))
        {
            mqttManager.PublishSnow(SettingsController.GetSnowTopic(), SettingsController.GetPlayerNo(), true);
            TakeDamage(5);
            snowStacks += 1;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OpponentSnow"))
        {
            mqttManager.PublishSnow(SettingsController.GetSnowTopic(), SettingsController.GetPlayerNo(), false);
            snowStacks -= 1;
        }
    }

    public int GetSnowDamage() 
    {
        return snowStacks * 5;
    }

}
