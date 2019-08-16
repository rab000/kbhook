using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ServerMgr : MonoBehaviour
{
    public static ServerMgr Ins;

    public Button StartBtn;

    public Text StartBtnText;

    public Text InfoText;

    void Awake()
    {
        Ins = this;
    }

    void Start()
    {
        StartBtnText.text = "start";
        InfoText.text = "wait";
    }

    void OnDestroy()
    {
        Ins = null;
    }

    void OnEnable()
    {
        StartBtn.onClick.AddListener(OnBtnClick);
        AsyncTCPServer.Ins.OnClientConnect += OnClientConnect;
    }

    void OnDisable()
    {
        StartBtn.onClick.RemoveListener(OnBtnClick);
        AsyncTCPServer.Ins.OnClientConnect -= OnClientConnect;
    }

    void OnBtnClick()
    {
        if (ServerMgr.Ins.BeServerConnect())
        {
            ServerMgr.Ins.CloseServer();

            StartBtnText.text = "start";
        }
        else
        {
            StartListen(() =>
            {
                StartBtnText.text = "stop";
            });
        }

    }

    void OnClientConnect(string clientIP)
    {
        InfoText.text = "client " + clientIP + " connect!!!";
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
