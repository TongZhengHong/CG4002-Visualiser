using System;
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
    private MqttManager mqttManager;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tagMqtt).Length == 0)
        {
            Debug.LogError("Error finding MqttManager from MqttController.");
            return;
        }
        mqttManager = GameObject.FindGameObjectsWithTag(tagMqtt)[0].gameObject.GetComponent<MqttManager>();
        mqttManager.OnMessageArrived += OnMessageArrivedHandler;
        mqttManager.OnConnectionSucceeded += OnConnectionChanged;
    }

    void OnDisable()
    {
        if (mqttManager != null)
        {
            mqttManager.OnMessageArrived -= OnMessageArrivedHandler;
            mqttManager.OnConnectionSucceeded -= OnConnectionChanged;
        }
    }

    private void OnMessageArrivedHandler(MqttObj mqttObject) //MqttObj is defined in MqttManager.cs
    {
        string actionTopic = SettingsController.GetActionTopic();
        if (mqttObject.topic != actionTopic) return;

        string[] actionStrings = { "None", "Shoot", "Shield", "Reload", "Logout", 
            "Bomb", "Badminton", "Golf", "Fencing", "Boxing" };
        receivedText.text = "Received: \n" + actionStrings[mqttObject.payload];
        receivedText.text += " for Player " + mqttObject.playerNo.ToString();
    }

    private void OnConnectionChanged(bool isConnected) 
    {
        isConnectedText.text = "MQTT Status: " + (isConnected ? "Connected" : "Disconnected");
        connectButtonText.text = isConnected ? "Disconnect" : "Connect";
    }

}