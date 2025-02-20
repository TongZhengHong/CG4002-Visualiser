using System.Collections;
using UnityEngine;
using TMPro;

public class ShieldController : MonoBehaviour
{

    [SerializeField] private GameObject playerShield;

    [SerializeField] private GameObject shieldCountObject;

    private ShieldBombSection shieldBombSection;

    private int maxShield = 3;

    private int shieldCount = 0;

    private Animator shieldAnimator;

    void Awake()
    {
        shieldCount = maxShield;
        shieldAnimator = playerShield.GetComponent<Animator>();

        PlayerInfo.ShieldDestroyed += OnShieldDestroy;

        if (shieldCountObject != null) 
        {
            shieldBombSection = shieldCountObject.GetComponent<ShieldBombSection>();
        }
    }

    private void OnShieldDestroy() 
    {
        StartCoroutine(CloseShield());
    }

    public void OnRespawn() 
    {
        shieldCount = maxShield;
        UpdateShieldCount(maxShield);
        StartCoroutine(CloseShield());
    }

    public IEnumerator CloseShield()
    {
        if (playerShield.activeSelf) 
        {
            shieldAnimator.SetTrigger("CloseShield");
            yield return new WaitForSeconds(1f);
            playerShield.SetActive(false);
        }
    }

    public bool ActivateShield()
    {
        if (playerShield.activeSelf) return false;
        if (shieldCount == 0) 
        {
            UpdateShieldCount(-1);
            return false;
        }

        shieldCount--;
        playerShield.SetActive(true);
        shieldAnimator.SetTrigger("OpenShield");
        UpdateShieldCount(shieldCount);

        return true;
    }

    private void UpdateShieldCount(int count)
    {
        if (shieldBombSection != null) 
        {
            shieldBombSection.UpdateShieldCount(count);
        }
    }

}
