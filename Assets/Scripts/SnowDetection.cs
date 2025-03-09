using UnityEngine;

public class SnowDetection : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name} entered the trigger!");
    }

}
