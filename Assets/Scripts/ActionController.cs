using UnityEngine;

public class ActionController : MonoBehaviour
{

    [SerializeField] private GameObject shuttlecockPrefab;
    [SerializeField] private GameObject golfPrefab;

    private Camera cam;

    private Vector3 camPos;

    private Quaternion camRotation;

    void Start()
    {
        cam = Camera.main;

        MqttController.GolfTrigger += OnGolfTriggered;
        MqttController.BadmintonTrigger += OnBadmintonTriggered;
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

}
