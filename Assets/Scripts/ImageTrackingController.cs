using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingController : MonoBehaviour
{

    [Header("Reference Image Names")]
    [SerializeField] private string qrCodeName = "qr_code";

    [SerializeField] private string targetImageName = "target_image";

    [Header("Game Objects")]
    [SerializeField] private GameObject opponent;

    [SerializeField] private GameObject reticlePointer;

    [SerializeField] private GameObject referencePlane;

    private ARTrackedImageManager trackedImagesManager;

    void Awake() => trackedImagesManager = FindFirstObjectByType<ARTrackedImageManager>();

    void OnEnable() => trackedImagesManager.trackablesChanged.AddListener(OnChanged);

    void OnDisable() => trackedImagesManager.trackablesChanged.RemoveListener(OnChanged);

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added) {}

        foreach (var updatedImage in eventArgs.updated)
        {
            if (updatedImage.referenceImage.name == qrCodeName) 
            {
                referencePlane.SetActive(true);
                referencePlane.transform.position = updatedImage.transform.position;
            }
            else if (updatedImage.referenceImage.name == targetImageName)
            {
                opponent.SetActive(updatedImage.trackingState == TrackingState.Tracking);
                opponent.transform.position = updatedImage.transform.position;
                Debug.Log("Opponent position: " + opponent.transform.position);
                opponent.transform.rotation = Quaternion.LookRotation(updatedImage.transform.up, Vector3.up);
            }
        }
    }

}
