using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MqttController : MonoBehaviour
{
    private string tagMqtt = "MQTT";
    private MqttManager _eventSender;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tagMqtt).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tagMqtt)[0].gameObject.GetComponent<MqttManager>();
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
        }
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(MqttObj mqttObject) //MqttObj is defined in MqttManager.cs
    {
        // Might want to filter the correct topic here
        Debug.Log("Message, from Topic " + mqttObject.topic + " is = " + mqttObject.msg);
    }
}