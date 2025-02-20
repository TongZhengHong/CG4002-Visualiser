using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleFlash;

    [SerializeField] private GameObject mazagineObject;

    private MazagineController mazagineController;

    [SerializeField] private float fireRate = 5f; // Bullets per seconds

    [SerializeField] public const float RELOAD_TIME = 3f; // in seconds 

    private Animator gunAnimator;

    public const int MAX_BULLETS = 6;

    private int bulletCount = 6;

    private float timeSinceLastShot = 0f;

    private bool isReloading = false;

    private int BULLET_DAMAGE = 5;

    private void Start()
    {
        bulletCount = MAX_BULLETS;
        gunAnimator = GetComponent<Animator>();

        CameraMovementChecker.OnCameraMoved += UpdateMoveAnimation;

        if (mazagineObject != null)
        {
            mazagineController = mazagineObject.GetComponent<MazagineController>();
        }
    }

    public void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void UpdateMoveAnimation(bool isMoving)
    {
        gunAnimator.SetBool("isMoving", isMoving);
    }

    public int ShootGun()
    {
        if (!CanShoot()) return 0;

        if (bulletCount >= 0 && mazagineController != null) 
        { // Show empty mazagine animation
            mazagineController.ShootBullet();
        }

        if (bulletCount > 0) 
        { 
            bulletCount--;
            timeSinceLastShot = 0f;

            muzzleFlash.Play();
            gunAnimator.SetTrigger("Shoot");
            return BULLET_DAMAGE;
        } 

        return 0;
    }

    private bool CanShoot() => !isReloading && timeSinceLastShot > (1f / fireRate);  

    public void ReloadBullets()
    {
        if (!isReloading && bulletCount == 0)
        {
            StartCoroutine(Reload());
            if (mazagineController != null) 
            {
                mazagineController.ReloadBullets();
            }
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

    public void OnRespawn()
    {
        StopAllCoroutines();
        bulletCount = MAX_BULLETS;
        gunAnimator.SetBool("isMoving", false);
        gunAnimator.SetBool("isReloading", false);

        if (mazagineController != null)
        {
            mazagineController.OnRespawn();
        }
    }

}