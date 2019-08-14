using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Server Ins;

    void Awake()
    {
        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    public void StartListen()
    {
        SocketServer.Ins.StartServer(ProcessKBData);
    }

    private void ProcessKBData(byte[] bs)
    {

    }

}
