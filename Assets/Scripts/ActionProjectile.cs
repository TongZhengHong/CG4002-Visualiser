using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ActionProjectile: MonoBehaviour
{

    [Header("Curve Charateristics")]
    [Tooltip("Distance below the camera's position.")]
    [SerializeField] private float startingHeightOffset = 0.3f;

    [Tooltip("Overall height the projectile will reach from the ground.")]
    [SerializeField] private float projectileHeight = 1.5f;

    [Tooltip("Distance the projectile will cover. It will always reach the ground.")]
    [SerializeField] private float totalDistance = 2f;

    [Tooltip("Animation duration in seconds")]

    [SerializeField] private float duration = 3f;

    [Header("Animation Options")]
    [SerializeField] private bool useRandomRotation = false;
    [SerializeField] private bool rotateZAxis = false;

    [Header("Prefabs")]

    [SerializeField] private GameObject explosionPrefab;

    private Vector3 start = Vector3.zero;

    private Vector3 end = Vector3.zero;

    private Vector3 turningPoint = Vector3.zero;

    public Vector3 Evaluate(float t) 
    {
        if (start == null || end == null || turningPoint == null) return Vector3.zero;

        Vector3 first = Vector3.Lerp(start, turningPoint, t);
        Vector3 second = Vector3.Lerp(turningPoint, end, t);
        return Vector3.Lerp(first, second, t);
    }

    public void OnLaunchProjectile(bool isOppVisible, Vector3 endPosition)
    {
        start = transform.position;
        start.y -= startingHeightOffset; // Offset below eye/camera level
        start += transform.right * 0.2f; // Start projectile right of center 

        end = isOppVisible ? endPosition : start + transform.forward * totalDistance;
        // end.y = 0;

        Vector3 midPoint = (start + end) / 2;
        turningPoint = new Vector3(midPoint.x, midPoint.y + projectileHeight, midPoint.z);

        StopAllCoroutines();
        StartCoroutine(AnimateProjectile());
    }

    public IEnumerator AnimateProjectile()
    {
        float elapsed = 0f;
        float rotationSpeed = 300f; // degrees per second

        float xRotation = Random.Range(150f, 300f); 
        float yDirection = Random.Range(0, 1f) > 0.5 ? -1 : 1;
        float yRotation = Random.Range(100f, 200f) * yDirection;

        if (!useRandomRotation) 
        {
            xRotation = 0;
            yRotation = 0;
        }

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float zRotation = rotateZAxis ? rotationSpeed * Time.deltaTime : 0;

            transform.position = Evaluate(progress);
            transform.Rotate(xRotation * Time.deltaTime, yRotation * Time.deltaTime, zRotation);

            elapsed += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(TriggerExplosion());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Human"))
        {
            StartCoroutine(TriggerExplosion());
        }
    }

    private IEnumerator TriggerExplosion()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.Euler(Vector3.up));
        yield return new WaitForSeconds(0.4f);
        Destroy(explosion);
        Destroy(gameObject);
    } 

}
