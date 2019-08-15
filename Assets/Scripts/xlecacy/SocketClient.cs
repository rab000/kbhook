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

    Socket _socket;

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

        Debug.Log("Client SetIP :"+ip +" port:"+port);

    }

    public void StartConnect()
    {
        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ip = IPAddress.Parse(ServerIP);

            IPEndPoint point = new IPEndPoint(ip, ServerPort);

            _socket.Connect(point);

            Thread c_thread = new Thread(Received);

            c_thread.IsBackground = true;

            c_thread.Start();
        }
        catch(Exception e)
        {
            Debug.LogError("client socket error :"+e.ToString());
        }

    }

    public void SafeClose()
    {

        Debug.Log("start close client socket0");

        if (_socket == null)
            return;

        Debug.Log("start close client socket1");

        if (!_socket.Connected)
            return;

        Debug.Log("start close client socket2");

        try
        {
            _socket.Shutdown(SocketShutdown.Both);
        }
        catch (Exception e)
        {
            Debug.LogError("Client shutdown exp :" + e.ToString());
        }

        if (true)
        {
            return;
        }


        try
        {
            _socket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Client close exp :" + e.ToString());
        }
    }

    public void SendMsg(byte[] bs)
    {
        try
        {
            _socket.Send(bs);
        }
        catch(Exception e)
        {
            Debug.LogError("Client send exp :"+e.ToString());
        }
        
    }

    void Received()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024 * 3];
                //实际接收到的有效字节数
                int len = _socket.Receive(buffer);
                if (len == 0)
                {
                    break;
                }
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("客户端打印：" + _socket.RemoteEndPoint + ":" + str);
            }
            catch { }
        }
    }

   
}