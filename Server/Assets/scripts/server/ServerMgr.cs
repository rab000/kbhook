using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using WindowsInput;
using WindowsInput.Native;

public class ServerMgr : MonoBehaviour
{
    public static ServerMgr Ins;

    public Button StartBtn;

    public InputField IPInputField;

    public InputField PortInputField;

    public Dropdown IPDropdown;

    public Text StartBtnText;

    public Text InfoText;

    InputSimulator sim = new InputSimulator();

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
        AsyncTCPServer.Ins.OnProccessMsg += ProcessKBData;
    }

    void OnDisable()
    {
        StartBtn.onClick.RemoveListener(OnBtnClick);
        AsyncTCPServer.Ins.OnClientConnect -= OnClientConnect;
        AsyncTCPServer.Ins.OnProccessMsg -= ProcessKBData;
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

            string _ip = "";
            int _port = 0;

            if (!IPInputField.text.Equals("nil"))
            {
                _ip = IPInputField.text;
                _port = int.Parse(PortInputField.text);
            }
            else
            {
                _ip = IPDropdown.captionText.text;
                _port = int.Parse(PortInputField.text);
            }

            AsyncTCPServer.Ins.SetIPPort(_ip, _port);
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
  

    private void ProcessKBData(string msg)
    {
        Debug.Log("ServerMgr.ProcessKBData msg:"+msg);
        if (!msg.Contains("mouse"))
        {
            ProcessKB(msg);
        }
        else
        {
            ProcessMouse(msg);
        }

    }

    private void ProcessKB(string msg)
    {        
        sim.Keyboard.TextEntry(msg);
    }

    private void ProcessMouse(string msg)
    {
        string[] ss = msg.Split('|');
       
        string mouseKey = ss[1];

        switch (mouseKey)
        {
            case "mid":
                Debug.Log("mid ss2:" + ss[2] );
                //ss[2] offy;
                int scrollNum = -int.Parse(ss[2]);
                sim.Mouse.VerticalScroll(scrollNum);
                break;
            case "left":
                sim.Mouse.LeftButtonClick();
                break;
            case "right":
                sim.Mouse.RightButtonClick();
                break;
            case "move":
                //ss[2] offx
                //ss[3] offy
                Debug.Log("move ss2:"+ss[2]+" ss3:"+ss[3]);
                int offx = int.Parse(ss[2]);
                int offy = int.Parse(ss[3]);
                sim.Mouse.MoveMouseBy(offx,offy);
                break;

            default:

                break;
        }

        //sim

    }


}
