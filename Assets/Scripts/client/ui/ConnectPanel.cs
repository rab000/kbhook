using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectPanel : MonoBehaviour
{
    [SerializeField] InputField IP_InputField;

    [SerializeField] InputField Port_InputField;

    [SerializeField] Button ClientConnectBtn;

    [SerializeField] Button ServerStartBtn;

    void OnEnable()
    {
        ClientConnectBtn.onClick.AddListener(OnConnectClick);
        ServerStartBtn.onClick.AddListener(OnServerStartClick);
    }

    void OnDisable()
    {
        ClientConnectBtn.onClick.RemoveListener(OnConnectClick);
        ServerStartBtn.onClick.RemoveListener(OnServerStartClick);
    }


    public void OnConnectClick()
    {
        int port = int.Parse(Port_InputField.text);

        string ip = IP_InputField.text;

        Debug.Log("ConnectClick ip:"+ ip+" port:"+port);
       
        ClientMgr.Ins.Connect(ip,port);

    }

    public void OnServerStartClick()
    {
        if (ServerMgr.Ins.BeServerConnect())
        {
            ServerMgr.Ins.CloseServer();

            var text = ServerStartBtn.transform.Find("Text").GetComponent<Text>();

            text.text = "start server";

        }
        else
        {
            ServerMgr.Ins.StartListen();

            var text = ServerStartBtn.transform.Find("Text").GetComponent<Text>();

            text.text = "close server";
        }

        
    }

}
