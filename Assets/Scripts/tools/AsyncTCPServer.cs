using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class AsyncTCPServer
{

    private const int MAX_CONNECT_NUM = 2;

    private const string IP = "127.0.0.1";

    private const int PORT = 54321;

    public void Start(Action OnStart = null)
    {
        //创建套接字
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(Default.SERVER_IP), Default.SERVER_PORT);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //绑定端口和IP
        socket.Bind(ipe);
        //设置监听
        socket.Listen(MAX_CONNECT_NUM);
        //连接客户端
        AsyncAccept(socket);

        Debug.Log("服务段启动 端口6065");

        OnStart?.Invoke();

    }

    /// <summary>
    /// 连接到客户端
    /// </summary>
    /// <param name="socket"></param>
    private void AsyncAccept(Socket socket)
    {
        socket.BeginAccept(asyncResult =>
        {
            //获取客户端套接字

            Socket client = socket.EndAccept(asyncResult);

            Debug.Log(string.Format("客户端{0}请求连接...", client.RemoteEndPoint));

            AsyncSend(client, "服务器收到连接请求");

            AsyncSend(client, string.Format("欢迎你{0}", client.RemoteEndPoint));

            AsyncReveive(client);

        }, null);

    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="client"></param>
    private void AsyncReveive(Socket socket)
    {
        
        byte[] data = new byte[1024];
        try
        {
            //开始接收消息
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
            asyncResult =>
            {

                int length = socket.EndReceive(asyncResult);

                Debug.Log("---服务端接受长度:"+length);

                Debug.Log(string.Format("服务器收到:{0}", Encoding.UTF8.GetString(data)));

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

