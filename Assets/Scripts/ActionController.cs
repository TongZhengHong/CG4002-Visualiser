using System.Collections;
using UnityEngine;

public class ActionController : MonoBehaviour
{

    [SerializeField] private GameObject shuttlecockPrefab;
    [SerializeField] private GameObject golfPrefab;
    
    [SerializeField] private GameObject boxingObject;

    [SerializeField] private GameObject fencingObject;

    private Camera cam;

    private Vector3 camPos;

    private Quaternion camRotation;

    private int GOLF_DAMAGE = 10;
    private int BADMINTON_DAMAGE = 10;
    private int BOXING_DAMAGE = 10;
    private int FENCING_DAMAGE = 10;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        camPos = cam.transform.position;
        camRotation = cam.transform.rotation;
    }

    public int TriggerGolf()
    {
        if (golfPrefab == null) return 0;

        GameObject golfInstance = Instantiate(golfPrefab, camPos, camRotation);
        ActionProjectile golf = golfInstance.GetComponent<ActionProjectile>();
        golf.OnLaunchProjectile();
        return GOLF_DAMAGE;
    }

    public int TriggerBadminton()
    {
        if (shuttlecockPrefab == null) return 0;

        GameObject badmintonInstance = Instantiate(shuttlecockPrefab, camPos, camRotation);
        ActionProjectile badminton = badmintonInstance.GetComponent<ActionProjectile>();
        badminton.OnLaunchProjectile();
        return BADMINTON_DAMAGE;
    }

    public int TriggerBoxing()
    {
        if (boxingObject == null) return 0;

        boxingObject.SetActive(true);
        Animator boxingAnimator = boxingObject.GetComponent<Animator>();
        if (boxingAnimator != null)
        {
            boxingAnimator.SetTrigger("PunchTrigger");
        }
        StartCoroutine(HideBoxingGloves());
        return BOXING_DAMAGE;
    }

    public IEnumerator HideBoxingGloves()
    {
        yield return new WaitForSeconds(2f);
        boxingObject.SetActive(false);
    }

    public int TriggerFencing()
    {
        if (fencingObject == null) return 0;

        fencingObject.SetActive(true);
        Animator fencingAnimator = fencingObject.GetComponent<Animator>();
        if (fencingAnimator != null) 
        {
            fencingAnimator.SetTrigger("LungeSword");
        }
        StartCoroutine(HideFencingSword());
        return FENCING_DAMAGE;
    }

    public IEnumerator HideFencingSword()
    {
        yield return new WaitForSeconds(2f);
        fencingObject.SetActive(false);
    }

}
