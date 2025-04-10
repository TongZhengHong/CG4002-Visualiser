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
        if (shieldAnimator != null)
            StartCoroutine(CloseShield());
        else
            playerShield.SetActive(false);
    }

    public void OnRespawn() 
    {
        shieldCount = maxShield;
        UpdateShieldCount(maxShield);

        if (shieldAnimator != null)
            StartCoroutine(CloseShield());
        else
            playerShield.SetActive(false);
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
        if (shieldAnimator != null) {
            shieldAnimator.SetTrigger("OpenShield");
        }
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

    public void SyncShield(int shieldCount, int shieldHealth)
    {
        if (this.shieldCount != shieldCount)
        {
            this.shieldCount = shieldCount;
            UpdateShieldCount(shieldCount);
        }

        if (!playerShield.activeSelf && shieldHealth > 0)
        {
            playerShield.SetActive(true);
            if (shieldAnimator != null && playerShield.transform.localScale == Vector3.zero) {
                shieldAnimator.SetTrigger("OpenShield");
            }
        }
        else if (playerShield.activeSelf && shieldHealth == 0)
        {
            if (shieldAnimator != null && playerShield.transform.localScale != Vector3.zero)
                StartCoroutine(CloseShield());
            else
                playerShield.SetActive(false);
        }
    }

}
