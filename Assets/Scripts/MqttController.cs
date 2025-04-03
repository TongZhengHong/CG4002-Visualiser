using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MqttController : MonoBehaviour
{
    [SerializeField] private string backendReadyTopic = "backend/ready";

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

    private void OnMessageArrivedHandler(MqttObj mqttObject) 
    {
        string actionTopic = SettingsController.GetActionTopic();
        string backendStateTopic = SettingsController.GetBackendTopic();

        Debug.Log(mqttObject.topic + " " + mqttObject.ident);
        if (mqttObject.topic == backendStateTopic || mqttObject.topic == backendReadyTopic) 
        {
            if (mqttObject.topic == backendReadyTopic) // Reset count to trigger update 
            {
                receivedText.text = "Received:";
                previousStateCount = 0;
            }

            receivedText.text += "\n Update State " + mqttObject.ident.ToString() + " ";
            receivedText.text += mqttObject.ident > previousStateCount ? "Success" : "Fail";

            if (mqttObject.ident > previousStateCount) // Check next payload after previous
            {
                UpdateGameState(mqttObject);
                previousStateCount = mqttObject.ident;
            }
        } 

        if (mqttObject.topic != actionTopic) return;

        string[] actionStrings = { "None", "Shoot", "Shield", "Reload", "Logout", 
            "Bomb", "Badminton", "Golf", "Fencing", "Boxing" };
        receivedText.text = "Received: \n" + actionStrings[mqttObject.payload[0]];
        receivedText.text += " for Player " + mqttObject.ident.ToString();
    }

    private void UpdateGameState(MqttObj mqttObject)
    {
        int oneHealth = (int) mqttObject.payload[0];
        int oneBullets = (int) mqttObject.payload[1];
        int oneBomb = (int) mqttObject.payload[2];
        int oneShieldHealth = (int) mqttObject.payload[3];
        int oneDeaths = (int) mqttObject.payload[4];
        int oneShields = (int) mqttObject.payload[5];
        int twoHealth = (int) mqttObject.payload[6];
        int twoBullets = (int) mqttObject.payload[7];
        int twoBomb = (int) mqttObject.payload[8];
        int twoShieldHealth = (int) mqttObject.payload[9];
        int twoDeaths = (int) mqttObject.payload[10];
        int twoShields = (int) mqttObject.payload[11];

        if (SettingsController.GetPlayerNo() == 1) {
            player.SyncPlayerInfo(oneHealth, oneBullets, oneBomb, oneShieldHealth, oneDeaths, oneShields);
            opponentPlayer.SyncOpponentInfo(twoHealth, twoBullets, twoBomb, twoShieldHealth, twoDeaths, twoShields);
        } else if (SettingsController.GetPlayerNo() == 2) {
            player.SyncPlayerInfo(twoHealth, twoBullets, twoBomb, twoShieldHealth, twoDeaths, twoShields);
            opponentPlayer.SyncOpponentInfo(oneHealth, oneBullets, oneBomb, oneShieldHealth, oneDeaths, oneShields);
        }
    }

    private void OnConnectionChanged(bool isConnected) 
    {
        isConnectedText.text = "MQTT Status: " + (isConnected ? "Connected" : "Disconnected");
        connectButtonText.text = isConnected ? "Disconnect" : "Connect";
    }

}