using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using nsocket;
public class ClientMgr : MonoBehaviour
{

    [SerializeField] Transform ConnectPanelTrm;

    [SerializeField] Transform AppPanelTrm;

    public static ClientMgr Ins;

    void Awake()
    {
        Ins = this;        
    }

    void OnDestroy()
    {
        Ins = null;

        if (ClientSocketMgr.GetIns().BeConnect())
        {
            ClientSocketMgr.GetIns().Close("scn destroy");
        }
            

    }

    void Start()
    {
        SetState(ClientStateEnum.Connect);
    }

    public enum ClientStateEnum
    {
        NULL,
        Connect,
        Keyboard
    }

    ClientStateEnum curState = ClientStateEnum.NULL;

    ClientStateEnum preState = ClientStateEnum.NULL;

    public void SetState(ClientStateEnum state)
    {
        if (state == curState) return;

        preState = curState;
        StateExit(preState);
        curState = state;

        Debug.Log("SetState切换状态到:" + state);

        switch (curState)
        {
            case ClientStateEnum.Connect:
                ConnectPanelTrm.gameObject.SetActive(true);
                break;
            case ClientStateEnum.Keyboard:               
                AppPanelTrm.gameObject.SetActive(true);                
                break;
        }
    }

    public bool IsCurState(ClientStateEnum state)
    {
        if (curState == state) return true;
        else return false;
    }

    void StateExit(ClientStateEnum preState)
    {

        Debug.Log("StateExit执行退出状态:" + preState);
        switch (preState)
        {
            case ClientStateEnum.Connect:
                Debug.Log("hide panel Connect0");
                ConnectPanelTrm.gameObject.SetActive(false);
                Debug.Log("hide panel Connect1");
                break;
            case ClientStateEnum.Keyboard:

                break;
        }
    }

    public void Connect(string ip, int port)
    {
        ClientSocketMgr.GetIns().Connect(ip, port, () =>
        {
            SetState(ClientStateEnum.Keyboard);
        });       
    }

}