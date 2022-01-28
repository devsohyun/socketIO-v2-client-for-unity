using Firesplash.UnityAssets.SocketIO;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SocketMessages;

public class SocketSettings
{
    public string ipAddress;
    public string port;
}

public class SocketManager : MonoBehaviour
{
    [Header("Socket")]
    public string serverIP;
    public string serverPort;
    public bool isConnected;

    [Header("UI")]
    bool contentsLoaded;
    public InputField userNameInput;
    public GameObject connectionErrorPanel;
    public Button reloadButton;
    public GameObject socketSettingPanel;
    public InputField inputIPAddress;
    public InputField inputPort;

    [Header("Scripts Socket")]
    public SocketIOCommunicator sioCom;
    public SocketEmitMessageList socketEmitMessageList;
    SocketSettings socketSettings;

    public string _cmd;


    [Serializable]
    struct ItsMeData
    {
        public string name;
    }


    #region Standard Function

    void Awake() 
    {
        LoadSettings();
    }

    public void Update()
    {
        _cmd = userNameInput.text;

        // Display Error setting Pannel to configure IP adress of nodejs server
        if (isConnected && connectionErrorPanel.activeSelf)
        {
            connectionErrorPanel.SetActive(false);
        }
        else if (!isConnected && !connectionErrorPanel.activeSelf)
        {
            connectionErrorPanel.SetActive(true);
            inputIPAddress.placeholder.GetComponent<Text>().text = serverIP;
            inputPort.placeholder.GetComponent<Text>().text = serverPort;
            StartCoroutine(CheckForSocketSetting(0.2f));
        }
    }

    public void StartConnection()
    {
        // // connection (you don't need this if you use Auto Reconnect on Inspector)
        // sioCom.Instance.Connect();

        // socket listeners
        sioCom.Instance.On("connect", (string data) => {
            Debug.Log("LOCAL: Hey, we are connected!");

            isConnected = true;
            //  knock at the servers door
            sioCom.Instance.Emit("KnockKnock");
        });

        // Server disconnect
        sioCom.Instance.On("disconnect", (string payload) => {
            Debug.Log("Disconnected from server.");

            isConnected = false;
            connectionErrorPanel.SetActive(true);

        });
    }

    public void CheckConnection()
    {
        // Display Error setting Pannel to configure IP adress of nodejs server
        if (isConnected && !connectionErrorPanel.activeSelf)
        {
            Debug.Log("isConnected && !connectionErrorPanel.activeSelf)");
            connectionErrorPanel.SetActive(false);
            socketSettingPanel.SetActive(false);
        }
        else if (!isConnected && connectionErrorPanel.activeSelf)
        {
            Debug.Log("(!isConnected && connectionErrorPanel.activeSelf)");
            connectionErrorPanel.SetActive(true);
            socketSettingPanel.SetActive(true);
            inputIPAddress.placeholder.GetComponent<Text>().text = serverIP;
            inputPort.placeholder.GetComponent<Text>().text = serverPort;
        }
    }

    public IEnumerator CheckForSocketSetting(float _wait)
    {
        yield return new WaitForSeconds(_wait);
        if (isConnected)
        {
            connectionErrorPanel.SetActive(false);
        }
        else
        {
            connectionErrorPanel.SetActive(true);
        }
    }

    public void ReloadTheApp()
    {
        CheckConnection();
    }

    #endregion

    #region Socket Settings

    public void UpdateServerAddress()
    {
        sioCom.socketIOAddress = serverIP + ":" + serverPort;
        // Debug.Log("HERE-2: "+"server ip: "+serverIP + "server port: "+serverPort);
    }

    #endregion

    #region Socket Standard Listeners

    public void UserNameSendButton()
    {
        Debug.Log("user name send: " + _cmd);
        SendName(_cmd);
        userNameInput.text = "";
    }
    // send cmd to node
    public void SendName (string _cmd) 
    {
        PlayerEvent newPlayerEvent = new PlayerEvent();
        newPlayerEvent.cmd = _cmd;
        string json = JsonUtility.ToJson(newPlayerEvent);
        SendMessageToNode(socketEmitMessageList.playerMessage, json);
    }

    // Send user name to server
    public void SendMessageToNode(string _message, string _contents)
    {
        Debug.Log("messages: " + _message);
    
        ItsMeData me = new ItsMeData(){
            name = _cmd // name json data
        };
        sioCom.Instance.Emit("ThisIsData", JsonUtility.ToJson(me), false);
    }


    #endregion

    #region UI Socket Settings

    public void ChangeIpAddress(string _ip, string _port) 
    {
        serverIP = _ip;
        serverPort = _port;
    }

    public void UpdateIpAddress(string _ip, string _port)
    {
        SaveServerSettings(_ip, _port);
        connectionErrorPanel.SetActive(false);
        Debug.Log("HERE-1 UpdateIpAddress: "+_ip+":"+_port);
    }

    public void SaveSettingsButton()
    {
        string _newIp = inputIPAddress.text;
        if (_newIp == "")
            _newIp = inputIPAddress.placeholder.GetComponent<Text>().text;
        string _newPort = inputPort.text;
        if (_newPort == "")
            _newPort = inputPort.placeholder.GetComponent<Text>().text;
        UpdateIpAddress(_newIp, _newPort);
        Debug.Log("HERE-3: "+_newIp+":"+_newPort);
      
        socketSettingPanel.SetActive(false);
    }

    #endregion


    #region Player Prefebs

    public void LoadSettings()
    {
        socketSettings = new SocketSettings();

        if (PlayerPrefs.HasKey("ipAddress") && PlayerPrefs.HasKey("port"))
            ChangeIpAddress(PlayerPrefs.GetString("ipAddress"), PlayerPrefs.GetString("port"));
        else
            UpdateIpAddress("127.0.0.1", "8888");
        
        UpdateServerAddress();

        StartConnection();

        CheckConnection();
    } 

    public void SaveServerSettings(string _newIp, string _newPort)
    {
        socketSettings.ipAddress = _newIp;
        PlayerPrefs.SetString("ipAddress", _newIp);
        socketSettings.port = _newPort;
        PlayerPrefs.SetString("port", _newPort);
        Debug.Log("HERE-2 SaveServerSettings: "+_newIp+_newPort);
    }

    #endregion

}
