using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SocketClient : MonoBehaviour
{

    public string ServerIP;

    public int ServerPort;

    Socket socketSend;

    public static SocketClient Ins;

    void Awake()
    {
        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    public void SetIP(string ip, int port)
    {
        ServerIP = ip;

        ServerPort = port;

    }

    public void StartConnect()
    {
        try
        {
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ip = IPAddress.Parse(ServerIP);

            IPEndPoint point = new IPEndPoint(ip, ServerPort);

            socketSend.Connect(point);

            //Thread c_thread = new Thread(Received);

            //c_thread.IsBackground = true;

            //c_thread.Start();
        }
        catch(Exception e)
        {

        }

    }

    public void SendMsg(byte[] bs)
    {       
        socketSend.Send(bs);
    }

    //void Received()
    //{
    //    while (true)
    //    {
    //        try
    //        {
    //            byte[] buffer = new byte[1024 * 1024 * 3];
    //            //实际接收到的有效字节数
    //            int len = socketSend.Receive(buffer);
    //            if (len == 0)
    //            {
    //                break;
    //            }
    //            string str = Encoding.UTF8.GetString(buffer, 0, len);
    //            Debug.Log("客户端打印：" + socketSend.RemoteEndPoint + ":" + str);
    //        }
    //        catch { }
    //    }
    //}

   
}