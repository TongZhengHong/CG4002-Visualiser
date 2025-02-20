using UnityEngine;
using TMPro;

public class BombController: MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;

    [SerializeField] private GameObject bombCountObject;

    private ShieldBombSection shieldBombSection;

    private int maxBomb = 2;

    private int bombCount = 0;

    private Camera cam;

    private Vector3 camPos;

    private Quaternion camRotation;

    private int BOMB_DAMAGE = 5;

    void Start()
    {
        cam = Camera.main;
        bombCount = maxBomb;

        if (bombCountObject != null)
        {
            shieldBombSection = bombCountObject.GetComponent<ShieldBombSection>();
        }
    }

    void LateUpdate()
    {
        camPos = cam.transform.position;
        camRotation = cam.transform.rotation;
    }

    public int ThrowBomb()
    {
        if (bombCount == 0)
        {
            UpdateBombCount(-1);
            return 0;
        } 

        GameObject bombInstance = Instantiate(bombPrefab, camPos, camRotation);
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

}
