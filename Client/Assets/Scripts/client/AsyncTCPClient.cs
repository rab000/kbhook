using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class AsyncTCPClient
{

    public Socket socket;

    public static AsyncTCPClient Ins = new AsyncTCPClient();

    private Action OnConnectSucces;

    /// <summary>
    /// 连接到服务器
    /// </summary>
    public void AsynConnect(string ip="127.0.0.1",int port=54321,Action OnConnect=null)
    {
        
        Debug.Log("客户段开始连接 ip "+ ip +" port:"+Default.SERVER_PORT);
        //端口及IP
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
        //创建套接字
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //开始连接到服务器
        socket.BeginConnect(ipe, asyncResult =>
        {
            OnConnect?.Invoke();

            socket.EndConnect(asyncResult);

            Debug.Log("客户段开始连接成功，开始发送消息");
            //向服务器发送消息
            AsynSend("你好我是客户端");

            AsynSend("第一条消息");

            AsynSend("第二条消息");

            //接受消息
            AsynRecive(socket);

        }, null);

    }



    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="message"></param>
    public void AsynSend(string message)
    {

        if (socket == null || message == string.Empty) return;
        //编码
        byte[] data = Encoding.UTF8.GetBytes(message);
        try
        {
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成发送消息
                int length = socket.EndSend(asyncResult);
                Debug.Log(string.Format("客户端发送消息:{0}", message));
            }, null);
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("异常信息：{0}", ex.Message));
        }

    }



    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="socket"></param>
    public void AsynRecive(Socket socket)
    {
        byte[] data = new byte[1024];
        try
        {
            //开始接收数据
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
            asyncResult =>
            {

                int length = socket.EndReceive(asyncResult);

                Debug.Log(string.Format("客户端收到服务器消息:{0}", Encoding.UTF8.GetString(data)));

                AsynRecive(socket);

            }, null);

        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("异常信息：{0}", ex.Message));
        }
    }


}

