﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectPanel : MonoBehaviour
{
    [SerializeField] InputField IP_InputField;

    [SerializeField] Dropdown IP_Dropdown;

    [SerializeField] InputField Port_InputField;

    [SerializeField] Button ClientConnectBtn;

    void OnEnable()
    {
        ClientConnectBtn.onClick.AddListener(OnConnectClick);        
    }

    void OnDisable()
    {
        ClientConnectBtn.onClick.RemoveListener(OnConnectClick);       
    }


    public void OnConnectClick()
    {
        int port = int.Parse(Port_InputField.text);

        string ip = null;

        if (IP_InputField.text.Equals("nil"))
        {
            ip = IP_Dropdown.captionText.text;
        }
        else
        {
            ip = IP_InputField.text;
        }
      
        Debug.Log("ConnectClick ip:"+ ip+" port:"+port);
       
        ClientMgr.Ins.Connect(ip,port);

    }   

}
