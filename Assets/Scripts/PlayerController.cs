using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerController: MonoBehaviour
{
    [SerializeField] private int playerNumber;

    [SerializeField] private GameObject gunControllerObject;

    [SerializeField] private GameObject opponentPlayerObject;

    private PlayerController opponentPlayer;

    private GunController gunController;
    
    private PlayerInfo playerInfo;

    private ShieldController shieldController;

    private BombController bombController;

    private ActionController actionController;

    private string tagMqtt = "MQTT";

    private MqttManager mqttManager;

    private int currentAction = 1;

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

        if (opponentPlayerObject != null)
        {
            opponentPlayer = opponentPlayerObject.GetComponent<PlayerController>();
        }
    }

    void LateUpdate()
    {
        if (opponentPlayerObject != null)
        {
            opponentPlayer = opponentPlayerObject.GetComponent<PlayerController>();
        }
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

    public void PlayAction()
    {
        int damageDealt = processAction(currentAction);
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
        if (mqttObject.topic != "viz/trigger") return;
        if (mqttObject.playerNo != playerNumber) {
            // TODO: Check if opponent hit player with action
            // TODO: Calculate damage dealt to player
            // TODO: Make player take damage and respawn if needed
            return;
        }

        int damageDealt = processAction(mqttObject.action);

        if (mqttObject.playerNo == 1) {
            DealDamageToOpponent(damageDealt);
            PublishPlayerViz();
        }
    }

    private void DealDamageToOpponent(int damageDealt)
    {
        if (damageDealt > 0)
        {
            StopAllCoroutines();
            StartCoroutine(ShowHitMissText(ReticlePointer.isLookingAtQR ? "Hit!" : "Miss..."));
        }

        if (ReticlePointer.isLookingAtQR && opponentPlayer != null)
        {
            opponentPlayer.TakeDamage(damageDealt);
        } 
    }

    private int processAction(int action)
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

    private void PublishPlayerViz() 
    {
        if (mqttManager != null) 
        {
            Debug.Log("Sending: " + ReticlePointer.isLookingAtQR.ToString());
            mqttManager.Publish(ReticlePointer.isLookingAtQR.ToString()); //new byte[] { isLookingAtQR ? (byte) 1 : (byte) 0 }
        }
    }

}
