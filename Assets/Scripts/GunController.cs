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

    private bool _isGunMoving;

    private const string IS_MOVING_TRIG = "isMoving";
    private const string IS_RELOADING_TRIG = "isReloading";
    private const string SHOOT_TRIG = "Shoot";

    public bool isGunMoving {
        get { return _isGunMoving; }
        private set
        {
            if (_isGunMoving != value)
            {
                _isGunMoving = value;
                gunAnimator.SetBool(IS_MOVING_TRIG, _isGunMoving);
            }
        }
    }
    private Vector3 lastGunPosition;

    private float movementThreshold = 0.07f;

    private void Start()
    {
        bulletCount = MAX_BULLETS;
        gunAnimator = GetComponent<Animator>();

        lastGunPosition = transform.position; // Initialise starting gun position

        if (mazagineObject != null)
        {
            mazagineController = mazagineObject.GetComponent<MazagineController>();
        }
    }

    public void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Vector3 currentVelocity = (transform.position - lastGunPosition) / Time.deltaTime;
        isGunMoving = currentVelocity.magnitude > movementThreshold;
        lastGunPosition = transform.position;
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
            gunAnimator.SetBool(IS_MOVING_TRIG, false);
            gunAnimator.SetTrigger(SHOOT_TRIG);
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
        gunAnimator.SetBool(IS_MOVING_TRIG, false);
        gunAnimator.SetBool(IS_RELOADING_TRIG, true);

        yield return new WaitForSeconds(RELOAD_TIME);

        gunAnimator.SetBool(IS_RELOADING_TRIG, false);
        isReloading = false;
    }

    public void OnRespawn()
    {
        StopAllCoroutines();
        bulletCount = MAX_BULLETS;
        gunAnimator.SetBool(IS_MOVING_TRIG, false);
        gunAnimator.SetBool(IS_RELOADING_TRIG, false);

        if (mazagineController != null)
        {
            mazagineController.OnRespawn();
        }
    }

    public void SyncBullets(int bulletCount)
    {
        this.bulletCount = bulletCount; 
        if (mazagineController != null)
        {
            mazagineController.SyncBullets(bulletCount);
        }
    }

}