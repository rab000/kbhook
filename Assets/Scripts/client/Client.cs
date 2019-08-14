using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{

    [SerializeField] Transform ConnectPanelTrm;

    [SerializeField] Transform AppPanelTrm;

    public static Client Ins;

    void Awake()
    {
        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
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

        Debug.Log("SetState切换状态到:" + state);

        preState = curState;
        StateExit(preState);
        curState = state;

        switch (curState)
        {
            case ClientStateEnum.Connect:
                ConnectPanelTrm.gameObject.SetActive(true);
                break;
            case ClientStateEnum.Keyboard:
                ConnectPanelTrm.gameObject.SetActive(false);
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
                ConnectPanelTrm.gameObject.SetActive(true);
                break;
            case ClientStateEnum.Keyboard:

                break;
        }
    }

}