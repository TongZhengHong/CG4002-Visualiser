using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MqttController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text isConnectedText;

    [SerializeField]
    private TMP_Text connectButtonText;

    [SerializeField]
    private TMP_Text receivedText;

    private string tagMqtt = "MQTT";
    private MqttManager _eventSender;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tagMqtt).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tagMqtt)[0].gameObject.GetComponent<MqttManager>();
            _eventSender.OnMessageArrived += OnMessageArrivedHandler;
            _eventSender.OnConnectionSucceeded += OnConnectionChanged;
            ReticlePointer.OnGazeStateChanged += SendStateToMqtt;
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
        }
    }

    void OnDisable()
    {
        if (_eventSender != null)
        {
            _eventSender.OnMessageArrived -= OnMessageArrivedHandler;
            _eventSender.OnConnectionSucceeded -= OnConnectionChanged;
        }
        ReticlePointer.OnGazeStateChanged -= SendStateToMqtt;
    }

    private void OnMessageArrivedHandler(MqttObj mqttObject) //MqttObj is defined in MqttManager.cs
    {
        // Might want to filter the correct topic here
        Debug.Log("Message, from Topic " + mqttObject.topic + " is = " + mqttObject.msg);
        receivedText.text = mqttObject.msg;
    }

    private void OnConnectionChanged(bool isConnected) 
    {
        isConnectedText.text = "MQTT Status: " + (isConnected ? "Connected" : "Disconnected");
        connectButtonText.text = isConnected ? "Disconnect" : "Connect";
    }

    private void SendStateToMqtt(bool isGazing) 
    {
        if (_eventSender != null) 
        {
            _eventSender.Publish("Looking at QR code: " + isGazing.ToString());
        }
    }
}