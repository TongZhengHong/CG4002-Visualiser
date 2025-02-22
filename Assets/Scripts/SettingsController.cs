using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{

    public static int PlayerNo { get; private set; } = 1;

    // Used to temporarily store player no change while in settings panel before save clicked
    private static int prevPlayerNo = 1;

    private string tagMqtt = "MQTT";

    private MqttManager mqttManager;

    public static string PLAYER_NO = "PlayerNo";
    public static string BROKER_ADDRESS = "MQTT_BrokerAddress";
    public static string BROKER_PORT = "MQTT_BrokerPort";
    public static string MQTT_USERNAME = "MQTT_Username";
    public static string MQTT_PASSWORD = "MQTT_Password";
    public static string ACTION_TOPIC = "MQTT_ActionTopic";
    public static string VISIBILITY_TOPIC = "MQTT_VisibilityTopic";

    [Header("Default Settings")]
    [SerializeField] private int defaultPlayerNo = 1;
    [SerializeField] private string defaultBrokerAddress = "localhost";
    [SerializeField] private int defaultBrokerPort = 1883;
    [SerializeField] private string defaultMqttUsername = "";
    [SerializeField] private string defaultMqttPassword = "";
    [SerializeField] private string defaultActionTopic = "viz/trigger";
    [SerializeField] private string defaultVisibilityTopic = "viz/player_vis";

    [Header("UI Object References")]
    [SerializeField] private GameObject settingsPanelObject;
    [SerializeField] private TMP_InputField brokerAddressField;
    [SerializeField] private TMP_InputField brokerPortField;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField actionTopicField;
    [SerializeField] private TMP_InputField visibilityTopicField;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button closeButton;

    void Awake()
    {
        saveButton.onClick.AddListener(HandleSave);
        closeButton.onClick.AddListener(HandleClose);
    }

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag(tagMqtt).Length == 0)
        {
            Debug.LogError("Error finding MqttManager from SettingsController.");
            return;
        }
        mqttManager = GameObject.FindGameObjectsWithTag(tagMqtt)[0].gameObject.GetComponent<MqttManager>();
    }

    public static void HandlePlayerToggle(bool player)
    {
        prevPlayerNo = player ? 1 : 2;
    }

    private void HandleSave()
    {
        PlayerNo = prevPlayerNo;

        if (mqttManager == null) return;
        mqttManager.SetMqttSettings(
            brokerAddressField.text,
            int.TryParse(brokerPortField.text, out int port1) ? port1 : defaultBrokerPort,
            usernameField.text,
            passwordField.text,
            actionTopicField.text,
            visibilityTopicField.text
        );

        SavePlayerPrefs(
            PlayerNo,
            brokerAddressField.text,
            int.TryParse(brokerPortField.text, out int port2) ? port2 : defaultBrokerPort,
            usernameField.text,
            passwordField.text,
            actionTopicField.text,
            visibilityTopicField.text 
        );

        HandleClose();
    }

    private void HandleClose()
    {
        if (settingsPanelObject != null)
        {
            settingsPanelObject.SetActive(false);
        }
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanelObject != null)
        {
            settingsPanelObject.SetActive(true);
            LoadPlayerPrefs();
        }
    }

    public void SavePlayerPrefs(int playerNo, string address, int port, string username, 
        string password, string action, string visibility)
    {
        PlayerPrefs.SetInt(PLAYER_NO, playerNo);
        PlayerPrefs.SetString(BROKER_ADDRESS, address);
        PlayerPrefs.SetInt(BROKER_PORT, port);
        PlayerPrefs.SetString(MQTT_USERNAME, username);
        PlayerPrefs.SetString(MQTT_PASSWORD, password); 
        PlayerPrefs.SetString(ACTION_TOPIC, action); 
        PlayerPrefs.SetString(VISIBILITY_TOPIC, visibility); 

        PlayerPrefs.Save();
    }

    public void LoadPlayerPrefs()
    {
        PlayerNo = PlayerPrefs.GetInt(PLAYER_NO, defaultPlayerNo);

        string address = PlayerPrefs.GetString(BROKER_ADDRESS, defaultBrokerAddress);
        int port = PlayerPrefs.GetInt(BROKER_PORT, defaultBrokerPort);
        string username = PlayerPrefs.GetString(MQTT_USERNAME, defaultMqttUsername);
        string password = PlayerPrefs.GetString(MQTT_PASSWORD, defaultMqttPassword);
        string action = PlayerPrefs.GetString(ACTION_TOPIC, defaultActionTopic);
        string visibility = PlayerPrefs.GetString(VISIBILITY_TOPIC, defaultVisibilityTopic);

        brokerAddressField.text = address;
        brokerPortField.text = port.ToString();
        usernameField.text = username;
        passwordField.text = password;
        actionTopicField.text = action;
        visibilityTopicField.text = visibility;

        mqttManager.SetMqttSettings(address, port, username,
            password, action, visibility);

        Debug.Log($"Loaded MQTT Settings: {address}:{port}, User: {username}");
    }
}
