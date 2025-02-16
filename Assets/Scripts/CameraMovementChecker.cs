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

    public static event Action<bool> OnCameraMoved;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        isMoving = transform.position != lastPosition;
        lastPosition = transform.position;
    }
}
