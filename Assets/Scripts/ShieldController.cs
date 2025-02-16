using System;
using UnityEngine;
using TMPro;

public class ShieldController : MonoBehaviour
{

    [SerializeField] private GameObject playerShield;

    private int maxShield = 3;

    private int shieldCount = 0;

    public static event Action ActivateShield;

    public static event Action<int> UpdateShieldCount;

    void Awake()
    {
        shieldCount = maxShield;

        MqttController.ShieldTrigger += OnShieldTriggered;
        PlayerInfo.ShieldDestroyed += OnShieldDestroy;
        PlayerInfo.PlayerDeath += onRespawn;
    }

    private void OnShieldTriggered()
    {
        if (!playerShield.activeSelf)
        {
            if (shieldCount == 0)
            {
                UpdateShieldCount?.Invoke(-1);
                return;
            } 
            shieldCount--;
            playerShield.SetActive(true);

            ActivateShield?.Invoke();
            UpdateShieldCount?.Invoke(shieldCount);
        }
    }

    private void OnShieldDestroy() 
    {
        playerShield.SetActive(false);
    }

    private void onRespawn() 
    {
        shieldCount = maxShield;
        UpdateShieldCount?.Invoke(shieldCount);
        playerShield.SetActive(false);
    }

}
