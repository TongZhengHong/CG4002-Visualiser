using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfo : MonoBehaviour
{

    [SerializeField] private Slider healthSlider;

    [SerializeField] private Slider shieldSlider;

    [SerializeField] private TMP_Text healthText;

    [SerializeField] private TMP_Text shieldText;

    private int currentHealth = 0;

    private int currentShield = 0;

    private const int MAX_HEALTH = 100;

    private const int MAX_SHIELD = 30;

    public static event Action PlayerDeath;

    public static event Action ShieldDestroyed;

    void Start()
    {
        currentShield = 0;
        currentHealth = MAX_HEALTH;

        healthSlider.maxValue = MAX_HEALTH * 10;
        shieldSlider.maxValue = MAX_SHIELD * 10;

        healthSlider.minValue = 0;
        shieldSlider.minValue = 0; 

        SetHealthSlider(currentHealth);
        SetShieldSlider(currentShield);

        ShieldController.ActivateShield += OnShieldTriggered;
    }

    public void TakeRandomDamage()
    {
        int damage = UnityEngine.Random.Range(5, 30);
        TakeDamage(damage);
        Debug.Log(damage);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        if (currentShield > 0)
        {
            int extraDamage = damage - currentShield;
            currentShield = Math.Max(currentShield - damage, 0);
            SetShieldSlider(currentShield);

            if (currentShield == 0)
            {
                ShieldDestroyed?.Invoke();
            }
            damage = Math.Max(extraDamage, 0);
        }

        // Apply remaining damage to health
        currentHealth = Math.Max(currentHealth - damage, 0);
        SetHealthSlider(currentHealth);
        if (currentHealth == 0) 
        {
            PlayerDeath?.Invoke();
            currentHealth = MAX_HEALTH;
            SetHealthSlider(currentHealth);
        }
    }

    private void OnShieldTriggered()
    {
        currentShield = MAX_SHIELD;
        SetShieldSlider(MAX_SHIELD);
    }

    private void SetShieldSlider(int newVal)
    {
        if (newVal < 0 || newVal > MAX_SHIELD) return; 
        shieldText.text = currentShield.ToString();

        int minimum = (int) shieldSlider.maxValue / 10;
        shieldSlider.value = minimum + newVal * 9;
    }

    private void SetHealthSlider(int newVal)
    {
        if (newVal < 0 || newVal > MAX_HEALTH) return; 
        healthText.text = currentHealth.ToString();

        int minimum = (int) healthSlider.maxValue / 10;
        healthSlider.value = minimum + newVal * 9;
    }

}
