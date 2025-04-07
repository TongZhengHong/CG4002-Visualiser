using System.Collections;
using UnityEngine;

public class ActionController : MonoBehaviour
{

    [SerializeField] private GameObject shuttlecockPrefab;
    
    [SerializeField] private GameObject golfPrefab;
    
    [SerializeField] private GameObject boxingObject;

    [SerializeField] private GameObject fencingObject;

    [SerializeField] private Transform startingTransform;

    private int GOLF_DAMAGE = 10;
    private int BADMINTON_DAMAGE = 10;
    private int BOXING_DAMAGE = 10;
    private int FENCING_DAMAGE = 10;

    public int TriggerGolf(bool isVisible, Vector3 endPosition)
    {
        if (golfPrefab == null) return 0;

        GameObject golfInstance = Instantiate(golfPrefab, startingTransform.position, startingTransform.rotation);
        ActionProjectile golf = golfInstance.GetComponent<ActionProjectile>();
        golf.OnLaunchProjectile(isVisible, endPosition);
        return GOLF_DAMAGE;
    }

    public int TriggerBadminton(bool isVisible, Vector3 endPosition)
    {
        if (shuttlecockPrefab == null) return 0;

        GameObject badmintonInstance = Instantiate(shuttlecockPrefab, startingTransform.position, startingTransform.rotation);
        ActionProjectile badminton = badmintonInstance.GetComponent<ActionProjectile>();
        badminton.OnLaunchProjectile(isVisible, endPosition);
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
