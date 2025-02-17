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

    public static event Action SendVisibilityTrigger;

    public static event Action GunShootTrigger;

    public static event Action ReloadTrigger;

    public static event Action BombTrigger;

    public static event Action ShieldTrigger;

    public static event Action GolfTrigger;

    public static event Action BadmintonTrigger;
    
    public static event Action BoxingTrigger;
    public static event Action FencingTrigger;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tagMqtt).Length == 0)
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
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
        if (mqttObject.topic != "viz/trigger") return;

        Debug.Log("Received: " + mqttObject.playerNo.ToString() + " " + mqttObject.action.ToString());

        receivedText.text = "Received: \n";
        switch (mqttObject.action)
        {
            case 1: // SHOOT
                GunShootTrigger?.Invoke();
                receivedText.text += "Shoot";
                break;
            case 2: // SHIELD
                ShieldTrigger?.Invoke();
                receivedText.text += "Shield";
                break;
            case 3: // RELOAD
                ReloadTrigger?.Invoke();
                receivedText.text += "Reload";
                break; 
            case 4: // LOGOUT
                receivedText.text += "Logout";
                break; 
            case 5: // BOMB
                BombTrigger?.Invoke();
                receivedText.text += "Bomb";
                break; 
            case 6: // BADMINTON
                BadmintonTrigger?.Invoke();
                receivedText.text += "Badminton";
                break; 
            case 7: // GOLF
                GolfTrigger?.Invoke();
                receivedText.text += "Golf";
                break; 
            case 8: // FENCING
                FencingTrigger?.Invoke();
                receivedText.text += "Fencing";
                break; 
            case 9: // BOXING
                BoxingTrigger?.Invoke();
                receivedText.text += "Boxing";
                break;  
            default:
                receivedText.text += "None";
                break; 
        }
        SendVisibilityTrigger?.Invoke();
    }

    private void OnConnectionChanged(bool isConnected) 
    {
        isConnectedText.text = "MQTT Status: " + (isConnected ? "Connected" : "Disconnected");
        connectButtonText.text = isConnected ? "Disconnect" : "Connect";
    }

}