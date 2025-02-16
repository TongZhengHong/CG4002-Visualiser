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
    private MqttManager _eventSender;

    public static event Action GunShootTrigger;

    public static event Action ReloadTrigger;

    public static event Action BombTrigger;

    public static event Action ShieldTrigger;

    public static event Action GolfTrigger;

    public static event Action BadmintonTrigger;

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
        if (mqttObject.topic != "backend/action") return;
        Debug.Log("Received: " + mqttObject.playerNo.ToString() + " " + mqttObject.action.ToString());
        switch (mqttObject.action)
        {
            case 1: // SHOOT
                GunShootTrigger?.Invoke();
                break;
            case 2: // SHIELD
                ShieldTrigger?.Invoke();
                break;
            case 3: // RELOAD
                ReloadTrigger?.Invoke();
                break; 

            case 4: // LOGOUT
           
                break; 
            case 5: // BOMB
                BombTrigger?.Invoke();
                break; 
            case 6: // BADMINTON
                BadmintonTrigger?.Invoke();
                break; 
            case 7: // GOLF
                GolfTrigger?.Invoke();
                break; 
            case 8: // FENCING
           
                break; 
            case 9: // BOXING
                         
                break;  
            default:
                       
                break; 
        }
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