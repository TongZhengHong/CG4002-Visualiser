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

    void Start()
    {
        cam = Camera.main;

        MqttController.GolfTrigger += OnGolfTriggered;
        MqttController.BadmintonTrigger += OnBadmintonTriggered;
        MqttController.BoxingTrigger += OnBoxingTriggered;
        MqttController.FencingTrigger += OnFencingTriggered;
    }

    void LateUpdate()
    {
        camPos = cam.transform.position;
        camRotation = cam.transform.rotation;
    }

    private void OnGolfTriggered()
    {
        if (golfPrefab == null) return;

        GameObject golfInstance = Instantiate(golfPrefab, camPos, camRotation);
        ActionProjectile golf = golfInstance.GetComponent<ActionProjectile>();
        golf.OnLaunchProjectile();
    }

    private void OnBadmintonTriggered()
    {
        if (shuttlecockPrefab == null) return;

        GameObject badmintonInstance = Instantiate(shuttlecockPrefab, camPos, camRotation);
        ActionProjectile badminton = badmintonInstance.GetComponent<ActionProjectile>();
        badminton.OnLaunchProjectile();
    }

    private void OnBoxingTriggered()
    {
        if (boxingObject == null) return;

        boxingObject.SetActive(true);
        Animator boxingAnimator = boxingObject.GetComponent<Animator>();
        if (boxingAnimator != null)
        {
            boxingAnimator.SetTrigger("PunchTrigger");
        }
        StartCoroutine(HideBoxingGloves());
    }

    public IEnumerator HideBoxingGloves()
    {
        yield return new WaitForSeconds(1.5f);
        boxingObject.SetActive(false);
    }

    private void OnFencingTriggered()
    {
        if (fencingObject == null) return;

        fencingObject.SetActive(true);
        Animator fencingAnimator = fencingObject.GetComponent<Animator>();
        if (fencingAnimator != null) 
        {
            fencingAnimator.SetTrigger("LungeSword");
        }
        StartCoroutine(HideFencingSword());
    }

    public IEnumerator HideFencingSword()
    {
        yield return new WaitForSeconds(1.5f);
        fencingObject.SetActive(false);
    }

}
