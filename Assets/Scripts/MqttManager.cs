using System;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;

// Store message received and topic subscribed, so we can select the right object from the controller
// C# GET/SET Property and event listener is used to reduce Update overhead in the controlled objects
public class MqttObj {
    public int playerNo;
    public int payload;

    private string m_topic;
    public string topic
    {
        get
        {
            return m_topic;
        }
        
        set
        {
            if (m_topic == value) return;
            m_topic = value;
        }
    }
}

public class MqttManager : M2MqttUnityClient
{
    public List<string> topicSubscribeList = new List<string>();

    private string topicPublish = "viz/player_vis";

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

    public void Publish(string topic, byte[] message)
    {
        if (client != null && client.IsConnected) 
        {
            client.Publish(topic, message, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log(message + " published on " + topicPublish);
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
        if (message.Length >= 2)
        {
            mqttObject.playerNo = message[0] == 0x01 ? 1 : 
                message[0] == 0x02 ? 2 : 0;
            int action = (int) message[1];
            mqttObject.payload = action > 9 ? 0 : action;
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
        string password, string action, string visibility)
    {
        brokerAddress = address;
        brokerPort = port;

        mqttUserName = username;
        mqttPassword = password;

        topicSubscribeList.Clear();
        topicSubscribeList.Add(action);
        topicSubscribeList.Add(visibility);

        topicPublish = visibility;
    }

}
