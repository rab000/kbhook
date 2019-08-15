using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServerMgr : MonoBehaviour
{
    public static ServerMgr Ins;

    void Awake()
    {
        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    public void StartListen(Action callback = null)
    {
        AsyncTCPServer.Ins.Start(callback);
    }

    public void CloseServer()
    {
        AsyncTCPServer.Ins.Close();
    }

    public bool BeServerConnect()
    {
        return AsyncTCPServer.BeListening;        
    }

    private void ProcessKBData(byte[] bs)
    {

    }

}
