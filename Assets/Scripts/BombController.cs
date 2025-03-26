using UnityEngine;
using TMPro;

public class BombController: MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;

    [SerializeField] private GameObject bombCountObject;

    [SerializeField] private Transform startingTransform;

    private ShieldBombSection shieldBombSection;

    private int maxBomb = 2;

    private int bombCount = 0;

    private int BOMB_DAMAGE = 5;

    void Start()
    {
        bombCount = maxBomb;

        if (bombCountObject != null)
        {
            shieldBombSection = bombCountObject.GetComponent<ShieldBombSection>();
        }
    }

    public int ThrowBomb()
    {
        if (bombCount == 0)
        {
            UpdateBombCount(-1);
            return 0;
        } 

        GameObject bombInstance = Instantiate(bombPrefab, startingTransform.position, startingTransform.rotation);
        ActionProjectile bomb = bombInstance.GetComponent<ActionProjectile>();
        bomb.OnLaunchProjectile();

        bombCount--;
        UpdateBombCount(bombCount);
        return BOMB_DAMAGE;
    }

    public void OnRespawn() 
    {
        bombCount = maxBomb;
        UpdateBombCount(maxBomb);
    }

    private void UpdateBombCount(int count)
    {
        if (shieldBombSection != null) 
        {
            shieldBombSection.UpdateBombCount(count);
        }
    }

    public void SyncBomb(int bombCount)
    {
        this.bombCount = bombCount;
        UpdateBombCount(bombCount);
    }

}
