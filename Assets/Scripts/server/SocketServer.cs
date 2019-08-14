﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

public class SocketServer : MonoBehaviour
{

    public Action<byte[]> ProcessData;

    private Dictionary<string, Session> SessionPool = new Dictionary<string, Session>();

    private Dictionary<string, string> MsgPool = new Dictionary<string, string>();

    private IPAddress IP;

    private int Port;

    private const int MAX_CLIENT_NUM = 1;

    public static SocketServer Ins;

    void Awake()
    {
        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    public void SetIP(string ip,string port)
    {
        try
        {

            IP = IPAddress.Parse(ip);

            Port = int.Parse(port);

        }
        catch
        {
            Debug.LogError("SetIP error ip:"+ip+" port:"+port);
        }
        
    }

    public void StartServer(Action<byte[]> processData)
    {
        ProcessData = processData;

        Socket SockeServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        SockeServer.Bind(new IPEndPoint(IP, Port));

        SockeServer.Listen(MAX_CLIENT_NUM);//客户端最大数

        SockeServer.BeginAccept(new AsyncCallback(Accept), SockeServer);

        Debug.Log("服务已启动");

    }

    //处理一个新的连接
    private void Accept(IAsyncResult socket)
    {
        // 还原传入的原始套接字
        Socket SockeServer = (Socket)socket.AsyncState;
        // 在原始套接字上调用EndAccept方法，返回新的套接字
        Socket SockeClient = SockeServer.EndAccept(socket);

        byte[] buffer = new byte[4096];
        try
        {
            //接收客户端的数据
            SockeClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), SockeClient);
            //保存登录的客户端
            Session session = new Session();
            session.SockeClient = SockeClient;
            session.IP = SockeClient.RemoteEndPoint.ToString();
            session.buffer = buffer;
            lock (SessionPool)
            {
                if (SessionPool.ContainsKey(session.IP))
                {
                    this.SessionPool.Remove(session.IP);
                }
                this.SessionPool.Add(session.IP, session);
            }
            //准备接受下一个客户端
            SockeServer.BeginAccept(new AsyncCallback(Accept), SockeServer);
            Debug.Log(string.Format("Client {0} connected", SockeClient.RemoteEndPoint));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error : " + ex.ToString());
        }
    }

    //处理一个连接的数据接收
    private void Recieve(IAsyncResult socket)
    {
        Socket SockeClient = (Socket)socket.AsyncState;
        string IP = SockeClient.RemoteEndPoint.ToString();
        if (SockeClient == null || !SessionPool.ContainsKey(IP))
        {
            return;
        }
        try
        {
            int length = SockeClient.EndReceive(socket);
            byte[] buffer = SessionPool[IP].buffer;
            SockeClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), SockeClient);
            //string msg = Encoding.UTF8.GetString(buffer, 0, length);

            //  websocket建立连接的时候，除了TCP连接的三次握手，websocket协议中客户端与服务器想建立连接需要一次额外的握手动作
            //if (msg.Contains("Sec-WebSocket-Key"))
            //{
            //    SockeClient.Send(PackageHandShakeData(buffer, length));
            //    SessionPool[IP].isWeb = true;
            //    return;
            //}
            //if (SessionPool[IP].isWeb)
            //{
            //    msg = AnalyzeClientData(buffer, length);
            //}

            
            //byte[] msgBuffer = PackageServerData(msg);
            //foreach (Session se in SessionPool.Values)
            //{
            //    se.SockeClient.Send(msgBuffer, msgBuffer.Length, SocketFlags.None);
            //}

            ProcessData?.Invoke(buffer);

        }
        catch
        {
            SockeClient.Disconnect(true);
            Console.WriteLine("客户端 {0} 断开连接", IP);
            SessionPool.Remove(IP);
        }
    }
    
    /*
    private byte[] PackageHandShakeData(byte[] handShakeBytes, int length)
    {
        string handShakeText = Encoding.UTF8.GetString(handShakeBytes, 0, length);
        string key = string.Empty;
        Regex reg = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
        Match m = reg.Match(handShakeText);
        if (m.Value != "")
        {
            key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
        }
        byte[] secKeyBytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
        string secKey = Convert.ToBase64String(secKeyBytes);
        var responseBuilder = new StringBuilder();
        responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + "\r\n");
        responseBuilder.Append("Upgrade: websocket" + "\r\n");
        responseBuilder.Append("Connection: Upgrade" + "\r\n");
        responseBuilder.Append("Sec-WebSocket-Accept: " + secKey + "\r\n\r\n");
        return Encoding.UTF8.GetBytes(responseBuilder.ToString());
    }

    private string AnalyzeClientData(byte[] recBytes, int length)
    {
        int start = 0;
        // 如果有数据则至少包括3位
        if (length < 2) return "";
        // 判断是否为结束针
        bool IsEof = (recBytes[start] >> 7) > 0;
        // 暂不处理超过一帧的数据
        if (!IsEof) return "";
        start++;
        // 是否包含掩码
        bool hasMask = (recBytes[start] >> 7) > 0;
        // 不包含掩码的暂不处理
        if (!hasMask) return "";
        // 获取数据长度
        UInt64 mPackageLength = (UInt64)recBytes[start] & 0x7F;
        start++;
        // 存储4位掩码值
        byte[] Masking_key = new byte[4];
        // 存储数据
        byte[] mDataPackage;
        if (mPackageLength == 126)
        {
            // 等于126 随后的两个字节16位表示数据长度
            mPackageLength = (UInt64)(recBytes[start] << 8 | recBytes[start + 1]);
            start += 2;
        }
        if (mPackageLength == 127)
        {
            // 等于127 随后的八个字节64位表示数据长度
            mPackageLength = (UInt64)(recBytes[start] << (8 * 7) | recBytes[start] << (8 * 6) | recBytes[start] << (8 * 5) | recBytes[start] << (8 * 4) | recBytes[start] << (8 * 3) | recBytes[start] << (8 * 2) | recBytes[start] << 8 | recBytes[start + 1]);
            start += 8;
        }
        mDataPackage = new byte[mPackageLength];
        for (UInt64 i = 0; i < mPackageLength; i++)
        {
            mDataPackage[i] = recBytes[i + (UInt64)start + 4];
        }
        Buffer.BlockCopy(recBytes, start, Masking_key, 0, 4);
        for (UInt64 i = 0; i < mPackageLength; i++)
        {
            mDataPackage[i] = (byte)(mDataPackage[i] ^ Masking_key[i % 4]);
        }
        return Encoding.UTF8.GetString(mDataPackage);
    }
    */

    /// <summary>
    /// 把发送给客户端消息打包处理（拼接上谁什么时候发的什么消息）
    /// </summary>
    /// <returns>The data.</returns>
    /// <param name="message">Message.</param>
    //private byte[] PackageServerData(string msg)
    //{
    //    byte[] content = null;
    //    byte[] temp = Encoding.UTF8.GetBytes(msg);
    //    if (temp.Length < 126)
    //    {
    //        content = new byte[temp.Length + 2];
    //        content[0] = 0x81;
    //        content[1] = (byte)temp.Length;
    //        Buffer.BlockCopy(temp, 0, content, 2, temp.Length);
    //    }
    //    else if (temp.Length < 0xFFFF)
    //    {
    //        content = new byte[temp.Length + 4];
    //        content[0] = 0x81;
    //        content[1] = 126;
    //        content[2] = (byte)(temp.Length & 0xFF);
    //        content[3] = (byte)(temp.Length >> 8 & 0xFF);
    //        Buffer.BlockCopy(temp, 0, content, 4, temp.Length);
    //    }
    //    return content;
    //}

}