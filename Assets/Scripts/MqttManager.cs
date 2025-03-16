using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;

// Store message received and topic subscribed, so we can select the right object from the controller
// C# GET/SET Property and event listener is used to reduce Update overhead in the controlled objects
public class MqttObj {
    public int ident;
    public byte[] payload;
    public string topic;
}

public class MqttManager : M2MqttUnityClient
{
    public List<string> topicSubscribeList = new List<string>();


    //new mqttObj is created to store message received and topic subscribed
    MqttObj mqttObject = new MqttObj();

    public event OnMessageArrivedDelegate OnMessageArrived;
    public delegate void OnMessageArrivedDelegate(MqttObj mqttObject);

    //using C# Property GET/SET and event listener to expose the connection status
    private bool m_isConnected = false;

    public bool isConnected
    {
        get
        {
            return m_isConnected;
        }

        set
        {
            if (m_isConnected == value) return;
            m_isConnected = value;
            if (OnConnectionSucceeded != null)
            {
                OnConnectionSucceeded(isConnected);
            }
        }
    }
    public event OnConnectionSucceededDelegate OnConnectionSucceeded;
    public delegate void OnConnectionSucceededDelegate(bool isConnected);

    private List<MqttObj> eventMessages = new List<MqttObj>();

    protected override void Start()
    {
        base.Start();

        (string address, int port, string username, string password, string action, 
            string visibility, string snow, string backend) = SettingsController.LoadPlayerPrefs();

        SetMqttSettings(address, port, username, password, action, visibility, snow, backend);
    }

    protected override void Update()
    {
        base.Update(); // call ProcessMqttEvents()
    }

    public void ToggleConnection() {
        if (!m_isConnected) {
            Connect();
        } else {
            Disconnect();
        }
    }

    public void Publish(string topic, string message)
    {
        Publish(topic, System.Text.Encoding.UTF8.GetBytes(message));
    }

    public void PublishVisibility(string topic, int playerNo, bool isVisible)
    {
        byte[] message = { Convert.ToByte(playerNo), Convert.ToByte(isVisible) };
        Publish(topic, message);
    }

    public void PublishSnow(string topic, int playerNo, bool inSnow)
    {
        byte[] message = { Convert.ToByte(playerNo), Convert.ToByte(inSnow) };
        Publish(topic, message);
    }

    public void Publish(string topic, byte[] message)
    {
        if (client != null && client.IsConnected) 
        {
            client.Publish(topic, message, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }
    }

    public void SetEncrypted(bool isEncrypted)
    {
        this.isEncrypted = isEncrypted;
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        isConnected = true;

        UnsubscribeTopics();
        SubscribeTopics();
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        Debug.Log("CONNECTION FAILED! " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        Debug.Log("Disconnected.");
        isConnected = false;
    }

    protected override void OnConnectionLost()
    {
        Debug.Log("CONNECTION LOST!");
    }

    protected override void SubscribeTopics()
    {
        if (client == null || !client.IsConnected) return;

        foreach (string item in topicSubscribeList)  
        {
            client.Subscribe(new string[] { item }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });   
        }
    }

    protected override void UnsubscribeTopics()
    {
        if (client == null || !client.IsConnected) return;
        
        foreach (string item in topicSubscribeList)
        {
            client.Unsubscribe(new string[] { item });
        }
    }

    protected override void DecodeMessage(string topicReceived, byte[] message)
    {
        string backendTopic = SettingsController.GetBackendTopic();
        if (message.Length >= 2)
        {
            if (topicReceived == backendTopic) {
                mqttObject.ident = (int) ((message[1] << 8) | message[0]);
                mqttObject.payload = message.Skip(2).ToArray();
            } else {
                mqttObject.ident = (int) message[0];
                mqttObject.payload = message.Skip(1).ToArray();
            }
        }

        mqttObject.topic = topicReceived;
        StoreMessage(mqttObject);
        
        if (OnMessageArrived != null) {
            OnMessageArrived(mqttObject);
        }
    }

    private void StoreMessage(MqttObj eventMsg)
    {
        if (eventMessages.Count > 50)
        {
            eventMessages.Clear();
        }
        eventMessages.Add(eventMsg);
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public void SetMqttSettings(string address, int port, string username, 
        string password, string action, string visibility, string snow, string backend)
    {
        brokerAddress = address;
        brokerPort = port;

        mqttUserName = username;
        mqttPassword = password;

        topicSubscribeList.Clear();
        topicSubscribeList.Add(action);
        topicSubscribeList.Add(visibility);
        topicSubscribeList.Add(snow);
        topicSubscribeList.Add(backend);

        Debug.Log($"Loaded MQTT Settings: {address}:{port}, User: {username}");
    }

}
