using System;
using UnityEngine;
using TMPro;

public class BombController: MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;

    private int maxBomb = 2;

    private int bombCount = 0;

    public static event Action<int> UpdateBombCount;

    private Camera cam;

    private Vector3 camPos;

    private Quaternion camRotation;

    void Start()
    {
        cam = Camera.main;
        bombCount = maxBomb;

        MqttController.BombTrigger += OnBombTriggered;
        PlayerInfo.PlayerDeath += onRespawn;
    }

    void LateUpdate()
    {
        camPos = cam.transform.position;
        camRotation = cam.transform.rotation;
    }

    private void OnBombTriggered()
    {
        if (bombCount == 0)
        {
            UpdateBombCount?.Invoke(-1);
            return;
        } 

        GameObject bombInstance = Instantiate(bombPrefab, camPos, camRotation);
        ActionProjectile bomb = bombInstance.GetComponent<ActionProjectile>();
        bomb.OnLaunchProjectile();

        bombCount--;
        UpdateBombCount?.Invoke(bombCount);
    }

    private void onRespawn() 
    {
        bombCount = maxBomb;
        UpdateBombCount?.Invoke(bombCount);
    }

}
