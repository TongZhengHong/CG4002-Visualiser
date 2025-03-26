using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MqttController : MonoBehaviour
{
    [SerializeField] private TMP_Text isConnectedText;

    [SerializeField] private TMP_Text connectButtonText;

    [SerializeField] private TMP_Text receivedText;

    [SerializeField] private GameObject playerObject;
    private PlayerController player;

    [SerializeField] private GameObject opponentObject;
    private OpponentController opponentPlayer;

    private string tagMqtt = "MQTT";
    private MqttManager mqttManager;

    private int previousStateCount = 0;

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

        opponentPlayer = opponentObject.GetComponent<OpponentController>();
        player = playerObject.GetComponent<PlayerController>();
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
        string backendTopic = SettingsController.GetBackendTopic();

        Debug.Log(mqttObject.topic + " " + mqttObject.ident);
        if (mqttObject.topic == backendTopic) 
        {
            receivedText.text += "\n Update State " + mqttObject.ident.ToString() + " ";
            receivedText.text += (mqttObject.ident > previousStateCount).ToString();

            if (mqttObject.ident > previousStateCount) // Check next payload after previous
            {
                int playerHealth = (int) mqttObject.payload[0];
                int playerBullets = (int) mqttObject.payload[1];
                int playerBomb = (int) mqttObject.payload[2];
                int playerShieldHealth = (int) mqttObject.payload[3];
                int playerDeaths = (int) mqttObject.payload[4];
                int playerShields = (int) mqttObject.payload[5];
                int opponentHealth = (int) mqttObject.payload[6];
                int opponentBullets = (int) mqttObject.payload[7];
                int opponentBomb = (int) mqttObject.payload[8];
                int opponentShieldHealth = (int) mqttObject.payload[9];
                int opponentDeaths = (int) mqttObject.payload[10];
                int opponentShields = (int) mqttObject.payload[11];

                player.SyncPlayerInfo(playerHealth, playerBullets, playerBomb, playerShieldHealth, playerDeaths, playerShields);
                opponentPlayer.SyncOpponentInfo(opponentHealth, opponentBullets, opponentBomb, opponentShieldHealth, opponentDeaths, opponentShields);

                previousStateCount = mqttObject.ident;
            }
        }

        if (mqttObject.topic != actionTopic) return;

        string[] actionStrings = { "None", "Shoot", "Shield", "Reload", "Logout", 
            "Bomb", "Badminton", "Golf", "Fencing", "Boxing" };
        receivedText.text = "Received: \n" + actionStrings[mqttObject.payload[0]];
        receivedText.text += " for Player " + mqttObject.ident.ToString();
    }

    private void OnConnectionChanged(bool isConnected) 
    {
        isConnectedText.text = "MQTT Status: " + (isConnected ? "Connected" : "Disconnected");
        connectButtonText.text = isConnected ? "Disconnect" : "Connect";
    }

}