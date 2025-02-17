using System;
using UnityEngine;

public class CameraMovementChecker : MonoBehaviour
{

    private bool _isMoving;

    public bool isMoving {
        get { return _isMoving; }
        private set
        {
            if (_isMoving != value)
            {
                _isMoving = value;
                OnCameraMoved?.Invoke(_isMoving); // Notify listeners
            }
        }
    }
    private Vector3 lastPosition;

    private float movementThreshold = 0.02f;

    public static event Action<bool> OnCameraMoved;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
        isMoving = currentVelocity.magnitude > movementThreshold;
        lastPosition = transform.position;
    }
}
