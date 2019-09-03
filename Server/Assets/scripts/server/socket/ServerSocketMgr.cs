using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace nsocket
{

    public class ServerSocketMgr
    {
        private const short MSG_HEAD = 127;
        //最大连接数
        private const int MAX_CONNECT_NUM = 2;

        //ip 注意一般不用127.0.0.1而是直接使用对外ip
        private string IP = "127.0.0.1";
        //private string IP = "10.0.115.239";

        private int Port = 5432;

        private static ServerSocketMgr Ins;

        private Socket serverSocket;

        public static bool BeListening = false;

        public Action<string> OnClientConnect;

        //以IP为key存储所有session
        public Dictionary<string, ServerSession> SessionDic = new Dictionary<string, ServerSession>();

        private ServerSocketMgr()
        {

        }

        public static ServerSocketMgr GetIns()
        {
            if (null == Ins) Ins = new ServerSocketMgr();

            return Ins;
        }

        public void SetIPPort(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        public void Start(Action OnStart = null)
        {
            //创建套接字
            IPEndPoint ipPort = new IPEndPoint(IPAddress.Parse(IP), Port);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定端口和IP
            serverSocket.Bind(ipPort);
            //设置监听
            serverSocket.Listen(MAX_CONNECT_NUM);

            serverSocket.NoDelay = true;

            BeListening = true;

            //连接客户端
            //AsyncAccept();

            Debug.Log("服务端开始监听 ip:" + IP + " port:" + Port);

            OnStart?.Invoke();

            //等待客户端连接
            serverSocket.BeginAccept(new AsyncCallback(AsyncAcceptClient), serverSocket);

        }

        public void Close()
        {
            //NINFO 关闭所有客户端连接，客户端单独下线的处理,这里的处理方法未验证
            foreach (var p in SessionDic)
            {
                CloseSession(p.Value);
            }
            SessionDic.Clear();

            //开始关闭服务器
            if (serverSocket == null)
            {
                Debug.LogError("SocketServer.Close 关闭失败  serverSocket == null");
                return;
            }

            if (!serverSocket.Connected)
            {
                Debug.LogError("SocketServer.Close 关闭失败  serverSocket.Connected == false");
                return;
            }

            try
            {
                serverSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Debug.LogError("SocketServer.Close Shutdown Exception :" + ex.ToString());
            }

            try
            {
                serverSocket.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError("SocketServer.Close Close Exception :" + ex.ToString());
            }

        }

        private void CloseSession(ServerSession session)
        {
            string ip = session.ID;

            var socket = session._SocketPackMgr._Socket;

            if (socket == null)
            {
                Debug.LogError("SocketServer.CloseSession 关闭失败  serverSocket == null");
                return;
            }

            Debug.Log("SocketServer.CloseSession ip:" + ip);

            if (!socket.Connected)
            {
                Debug.LogError("SocketServer.CloseSession 关闭失败  serverSocket.Connected == false");
                return;
            }

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Debug.LogError("SocketServer.CloseSession Shutdown Exception :" + ex.ToString());
            }

            try
            {
                socket.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError("SocketServer.CloseSession Close Exception :" + ex.ToString());
            }
        }

        #region rec

        private void AsyncAcceptClient(IAsyncResult asyncResult)
        {
            //获取客户端套接字

            Socket clientSocket = serverSocket.EndAccept(asyncResult);

            string clientIpPort = clientSocket.RemoteEndPoint.ToString();

            Debug.Log("SocketServer.AsyncAcceptClient 客户端连接:" + clientIpPort);

            Loom.QueueOnMainThread(() =>
            {

                OnClientConnect?.Invoke(clientIpPort);

            });

            Debug.Log(string.Format("SocketServer.AsyncAcceptClient 客户端{0}请求连接...", clientIpPort));

            NetworkStream networkSteam = new NetworkStream(clientSocket, false);

            ServerSession session = new ServerSession(clientIpPort, clientSocket, networkSteam);

            session._SocketPackMgr.OnProcessOnMsg = OnProcessOneMsg;

            if (!SessionDic.ContainsKey(session.ID))
            {
                SessionDic.Add(session.ID, session);
            }
            else
            {
                Debug.LogError("SocketServer.AsyncAcceptClient SessionPool中已经包含ip:" + session.ID + " 加入sessionPool失败");
            }

            Debug.Log("SocketServer.AsyncAcceptClient nmbnmb");

            //开始接受一个客户端的数据
            session._SocketPackMgr.ReadOnceFromSocket();

            //准备接受下一个客户端
            serverSocket.BeginAccept(new AsyncCallback(AsyncAcceptClient), serverSocket);

        }

        public static Queue<Cache4RecMsg> MsgQue = new Queue<Cache4RecMsg>();

        private void OnProcessOneMsg(Cache4RecMsg msg)
        {
            MsgQue.Enqueue(msg);
        }

        //注册接收消息
        public static Dictionary<short, Func<Cmd4Rec>> RecMsgDic = new Dictionary<short,  Func<Cmd4Rec>>();

        public static void RegistRecMsg(short cmd, Func<Cmd4Rec> recCmd)
        {
           
            if (!RecMsgDic.ContainsKey(cmd))
            {
                RecMsgDic.Add(cmd, recCmd);
            }
            else
            {
                Debug.LogError("ServerSocketMgr.RegistRecMsg faile 键值已存在，多次注册 cmd:" + cmd);
            }

        }

        #endregion

        #region send

        //ClientSendBuffer.PutShort(MSG_HEAD);
        //byte[] content = cmd.Encode();
        //int len = content.Length + 2;//+2是cmd的short长度            
        //ClientSendBuffer.PutInt(len);
        //ClientSendBuffer.PutShort(cmd.Cmd);
        //ClientSendBuffer.PutBytes(content);

        private static IoBuffer ServerSendBuffer = new IoBuffer(10240);

        public void Send(string sessionID, Cmd4Send cmd)
        {
            Debug.Log("服务端向sessionID:"+ sessionID +" 发送 cmd:"+cmd.Cmd);

            if (SessionDic.ContainsKey(sessionID))
            {
                ServerSendBuffer.Clear();
                ServerSendBuffer.PutShort(MSG_HEAD);
                byte[] content = cmd.Encode();
                int len = content.Length + 2;//+2是cmd的short长度
                ServerSendBuffer.PutInt(len);
                ServerSendBuffer.PutShort(cmd.Cmd);
                ServerSendBuffer.PutBytes(content);
                byte[] bs = ServerSendBuffer.ToArray();
                AsyncSend(SessionDic[sessionID]._SocketPackMgr._Socket,bs);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="p"></param>
        private void AsyncSend(Socket clientSocket, byte[] data)
        {
            if (clientSocket == null || null == data)
            {
                Debug.LogError("ServerSocketMgr.AsyncSend 失败  socket或 data = null");
                return;
            }
                
            try
            {
                //开始发送消息
                clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                //完成消息发送
                int length = clientSocket.EndSend(asyncResult);
                //输出消息
                //Debug.Log(string.Format("服务器发出消息:{0}", p));
                }, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        #endregion

        /// <summary>
        /// 连接到客户端
        /// </summary>
        /// <param name="socket"></param>
        //private void AsyncAccept3()
        //{
        //    serverSocket.BeginAccept(asyncResult =>
        //    {
        //        //获取客户端套接字

        //        Socket clientSocket = serverSocket.EndAccept(asyncResult);

        //        Loom.QueueOnMainThread(() => {

        //            OnClientConnect?.Invoke(clientSocket.RemoteEndPoint.ToString());

        //        });


        //        Debug.Log(string.Format("客户端{0}请求连接...", clientSocket.RemoteEndPoint));

        //        AsyncSend(clientSocket, "服务器收到连接请求");

        //        AsyncSend(clientSocket, string.Format("欢迎你{0}", clientSocket.RemoteEndPoint));

        //        //NTODO 这里是后补的，否则只能加一条消息
        //        AsyncReveive(clientSocket);

        //    }, null);

        //}
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client"></param>
        //private void AsyncReveive(Socket clientSocket)
        //{

        //    byte[] data = new byte[1024];
        //    try
        //    {
        //        //开始接收消息
        //        clientSocket.BeginReceive(data, 0, data.Length, SocketFlags.None,
        //        asyncResult =>
        //        {

        //            int length = clientSocket.EndReceive(asyncResult);

        //            Debug.Log("---服务端接受长度:" + length);

        //            string msg = Encoding.UTF8.GetString(data);

        //            Debug.Log(string.Format("服务器收到:{0}", msg));

        //            Loom.QueueOnMainThread(() =>
        //            {
        //                OnProccessMsg?.Invoke(msg);
        //            });

        //            AsyncReveive(clientSocket);

        //        }, null);

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //}
    }

}
