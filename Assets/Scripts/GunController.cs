using System;
using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleFlash;

    [SerializeField] private float fireRate = 5f; // Bullets per seconds

    [SerializeField] public const float RELOAD_TIME = 3f; // in seconds 

    private Animator gunAnimator;

    public const int MAX_BULLETS = 6;

    private int bulletCount = 6;

    private float timeSinceLastShot = 0f;

    private bool isReloading = false;

    public static event Action ReloadGunTrigger;

    public static event Action ShootGunTrigger;

    private void Start()
    {
        bulletCount = MAX_BULLETS;
        gunAnimator = GetComponent<Animator>();

        CameraMovementChecker.OnCameraMoved += UpdateMoveAnimation;
        MqttController.GunShootTrigger += ShootGun;
        MqttController.ReloadTrigger += ReloadBullets;
        PlayerInfo.PlayerDeath += OnRespawn;
    }

    public void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void UpdateMoveAnimation(bool isMoving)
    {
        gunAnimator.SetBool("isMoving", isMoving);
    }

    public void ShootGun()
    {
        if (!CanShoot()) return;

        if (bulletCount >= 0) 
        {
            ShootGunTrigger?.Invoke();
        }

        if (bulletCount > 0) 
        { 
            bulletCount--;
            timeSinceLastShot = 0f;

            muzzleFlash.Play();
            gunAnimator.SetTrigger("Shoot");
        } 
    }

    private bool CanShoot() => !isReloading && timeSinceLastShot > (1f / fireRate);  

    public void ReloadBullets()
    {
        if (!isReloading && bulletCount == 0)
        {
            StartCoroutine(Reload());
            ReloadGunTrigger?.Invoke();
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        bulletCount = MAX_BULLETS;
        gunAnimator.SetBool("isReloading", true);

        yield return new WaitForSeconds(RELOAD_TIME);

        gunAnimator.SetBool("isReloading", false);
        isReloading = false;
    }

    private void OnRespawn()
    {
        StopAllCoroutines();
        bulletCount = MAX_BULLETS;
        gunAnimator.SetBool("isMoving", false);
        gunAnimator.SetBool("isReloading", false);
    }

}