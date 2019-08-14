using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectPanel : MonoBehaviour
{

    [SerializeField] InputField IP_InputField;

    [SerializeField] InputField Port_InputField;

    [SerializeField] Button ConnectBtn;

    void OnEnable()
    {
        ConnectBtn.onClick.AddListener(OnConnectClick);
    }

    void OnDisable()
    {
        ConnectBtn.onClick.RemoveListener(OnConnectClick);
    }


    public void OnConnectClick()
    {
        int port = int.Parse(Port_InputField.text);

        Debug.Log("ConnectClick ip:"+ IP_InputField.text+" port:"+port);

        SocketClient.Ins.SetIP(IP_InputField.text,port);

        SocketClient.Ins.StartConnect();

        Client.Ins.SetState(Client.ClientStateEnum.Keyboard);
    }



}
