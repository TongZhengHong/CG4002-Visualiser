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

    // [SerializeField] private bool useFixedDistance = false;

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

    public void OnLaunchProjectile()
    {
        start = transform.position;
        start.y -= startingHeightOffset; // Offset below eye/camera level
        start += transform.right * 0.2f; // Start projectile right of center 
        end = start + transform.forward * totalDistance;

        Vector3 midPoint = (start + end) / 2;
        turningPoint = new Vector3(midPoint.x, midPoint.y + projectileHeight, midPoint.z);

        StopAllCoroutines();
        StartCoroutine(AnimateProjectile());
    }

    public IEnumerator AnimateProjectile()
    {
        float elapsed = 0f;
        float rotationSpeed = 300f; // degrees per second

        float xRotation = Random.Range(0.7f, 1.5f); 
        float yDirection = Random.Range(0, 1f) > 0.5 ? -1 : 1;
        float yRotation = Random.Range(0.5f, 0.1f) * yDirection; 

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
            transform.Rotate(xRotation, yRotation, zRotation);

            elapsed += Time.deltaTime;
            yield return null;
        }

        GameObject explosionObject = Instantiate(explosionPrefab, transform.position, Quaternion.Euler(Vector3.up));
        ParticleSystem[] explosionParticles = explosionObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in explosionParticles) ps.Play();

        yield return new WaitForSeconds(0.3f);
        Destroy(explosionObject);
        Destroy(gameObject);
    }

}
