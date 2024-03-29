﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class AsyncTCPServer
{

    private const int MAX_CONNECT_NUM = 2;

    //private const string IP = "127.0.0.1";
    private  string IP = "10.0.115.239";
    private  int Port = 54321;

    public static AsyncTCPServer Ins = new AsyncTCPServer();

    public Socket socket;

    public static bool BeListening = false;

    public Action<string> OnClientConnect;

    public Action<string> OnProccessMsg;

    public void Start(Action OnStart = null)
    {
        //创建套接字
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(IP), Port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //绑定端口和IP
        socket.Bind(ipe);
        //设置监听
        socket.Listen(MAX_CONNECT_NUM);

        BeListening = true;

        //连接客户端
        AsyncAccept();

        Debug.Log("服务端开始监听 ip:"+ IP+" port:"+Port);

        OnStart?.Invoke();

    }

    public void SetIPPort(string ip,int port)
    {
        IP = ip;
        Port = port;
    }

    public void Close()
    {
        //NTODO close server
    }

    /// <summary>
    /// 连接到客户端
    /// </summary>
    /// <param name="socket"></param>
    private void AsyncAccept()
    {
        socket.BeginAccept(asyncResult =>
        {
            //获取客户端套接字

            Socket clientSocket = socket.EndAccept(asyncResult);

            Loom.QueueOnMainThread(()=> {
                OnClientConnect?.Invoke(clientSocket.RemoteEndPoint.ToString());
            });
            

            Debug.Log(string.Format("客户端{0}请求连接...", clientSocket.RemoteEndPoint));

            AsyncSend(clientSocket, "服务器收到连接请求");

            AsyncSend(clientSocket, string.Format("欢迎你{0}", clientSocket.RemoteEndPoint));

            //NTODO 这里是后补的，否则只能加一条消息
            AsyncReveive(clientSocket);

        }, null);

    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="client"></param>
    private void AsyncReveive(Socket clientSocket)
    {
        
        byte[] data = new byte[1024];
        try
        {
            //开始接收消息
            clientSocket.BeginReceive(data, 0, data.Length, SocketFlags.None,
            asyncResult =>
            {

                int length = clientSocket.EndReceive(asyncResult);

                Debug.Log("---服务端接受长度:"+length);

                string msg = Encoding.UTF8.GetString(data);

                Debug.Log(string.Format("服务器收到:{0}", msg));

                Loom.QueueOnMainThread(() => {
                    OnProccessMsg?.Invoke(msg);
                });
                

                AsyncReveive(clientSocket);

            }, null);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="client"></param>
    /// <param name="p"></param>
    private void AsyncSend(Socket client, string p)
    {
        if (client == null || p == string.Empty) return;
        //数据转码
        byte[] data = new byte[1024];
        data = Encoding.UTF8.GetBytes(p);
        try
        {
            //开始发送消息
            client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成消息发送
                int length = client.EndSend(asyncResult);
                //输出消息
                Debug.Log(string.Format("服务器发出消息:{0}", p));
            }, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }

}

