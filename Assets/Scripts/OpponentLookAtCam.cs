using UnityEngine;

public class OpponentLookAtCam : MonoBehaviour
{

    void LateUpdate()
    {
        if (gameObject.activeSelf) 
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
}
