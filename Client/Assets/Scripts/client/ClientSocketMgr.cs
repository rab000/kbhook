using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System.IO;

namespace nsocket
{
    public class ClientSocketMgr
    {

        private const short MSG_HEAD = 127; 

        #region ins

        private static ClientSocketMgr Ins;

        private ClientSocketMgr()
        {

        }

        public static ClientSocketMgr GetIns()
        {
            if (null == Ins)
                Ins = new ClientSocketMgr();
            return Ins;
        }

        #endregion

        #region socket

        private Socket _Socket;

        private NetworkStream _NetworkStream;

        public Action OnConnectSuccess;

        public Action<byte[]> OnRecMsg;

        public void Connect(string ip, int port, Action onConnectSuccess)
        {

            Debug.Log("SocketClient.Connect ip:" + ip + " port:" + port);

            this.OnConnectSuccess = onConnectSuccess;

            try
            {
                if (BeConnect())
                {
                    System.Threading.Thread.Sleep(10);

                    this.Close("ReConnectServer");

                    System.Threading.Thread.Sleep(200);
                }

                IPEndPoint remoteServer = new IPEndPoint(IPAddress.Parse(ip), port);

                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //设置为true会延迟累积发送，但是会增加延迟
                //关闭Nagle算法
                //这里对于网络延迟敏感的应该设置为true，如果设置为ture，可能需要跟server频繁交互
                //否则连接后不交互，等一段时间Stream.EndRead就会一直读取到0，不停循环读取
                _Socket.NoDelay = true;

                _Socket.BeginConnect(remoteServer, OnConnect, null);

            }
            catch (System.Exception ex)
            {
                // 将断开连接消息压到解码队列
                //QueueDisconnect();
                Close(ex.Message);

                Debug.LogError("网络链接异常：" + ex.Message);
                //Cmd4Rec cmd = new Cmd4Rec();
                //cmd.MsgID = HEAD_MSG;
                //cmd.Content = string.Format("error {0}", ex.Message);
                //AbsNetMgr.Cmd4RecQueue.Enqueue(cmd);
                //NetWorkMessageResp.AddCommandSys (cmd);

            }

        }

        public void Close(string closeType)
        {
            if (_Socket == null)
            {
                Debug.LogError("SocketClient.Close_Socket==null closeSocket失败");

                return;
            }

            //NINFO 关闭网络发送，避免关闭后还有消息发出，导致异常
            //lock (this)
            //{
            //    BeMsgSending = false;
            //    SendMsgQueue.Clear();
            //}

            if (BeConnect())
            {
                try
                {
                    Debug.Log("SocketClient.Close shutdown!");
                    _Socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Debug.Log("SocketClient.Close socket.Shutdown exception：" + ex.ToString());
                }

                try
                {
                    _Socket.Close();
                    _Socket = null;
                    Debug.Log("SocketClient.Close close!");
                }
                catch (Exception ex)
                {

                    Debug.Log("SocketClient.Close socket.Close exception：" + ex.ToString());
                }

                _SocketDataMgr = null;

            }

        }

        public bool BeConnect()
        {
            if (_Socket == null)
                return false;
            return _Socket.Connected;
        }

        //连接(成功)后操作
        private void OnConnect(System.IAsyncResult ar)
        {
            try
            {
                if (_Socket == null || !_Socket.Connected)
                {
                    Debug.Log("SocketClient.OnConnect tcp connect faile! ");

                    return;

                }

                Debug.Log("SocketClient.OnConnect tcp connect success!");

                _Socket.EndConnect(ar);

                _NetworkStream = new NetworkStream(_Socket, false);

                _SocketDataMgr = new SocketPackMgr(_Socket, _NetworkStream);

                _SocketDataMgr.CloseEvent = Close;

                _SocketDataMgr.OnProcessOnMsg = OnProcessOneMsg;

                Loom.QueueOnMainThread(() =>
                {
                    OnConnectSuccess?.Invoke();
                });

                _SocketDataMgr.ReadOnceFromSocket();



            }
            catch (Exception e)
            {
                Debug.LogError("SocketClient.OnConnect exception:" + e.Message);

                Close("OnConnect exception:" + e.Message);
            }
            finally
            {
                //if (!_Socket.Connected)
                //{
                //NaTodo 内部消息
                ////####之后添加
                //XCKuaFuMessage.AutoLogin msg = new XCKuaFuMessage.AutoLogin();
                //msg.cmd = FID.CC_KUAFU_AUTOLOGIN;
                //msg.bIsReady = true;
                //NetWorkMessageResp.cmdlist.Enqueue(msg);
                //}
            }

        }

        #endregion

        #region send

        private bool BeMsgSending = false;

        private double sendMsgLen = 0;

        private Queue<byte[]> SendMsgQueue = new Queue<byte[]>();

        private static IoBuffer ClientSendBuffer = new IoBuffer(10240);

        public void Send(Cmd4Send cmd)
        {
            ClientSendBuffer.Clear();
            ClientSendBuffer.PutShort(MSG_HEAD);
            byte[] content = cmd.Encode();
            int len = content.Length + 2;//+2是cmd的short长度            
            ClientSendBuffer.PutInt(len);
            ClientSendBuffer.PutShort(cmd.Cmd);
            ClientSendBuffer.PutBytes(content);
            byte[] bs = ClientSendBuffer.ToArray();

            //Debug.Log("发送长度:"+bs.Length);

            //IoBuffer ib = new IoBuffer(102400);
            //ib.PutBytes(bs);
            //short head = ib.GetShort();
            //int len0 = ib.GetInt();
            //int cmd0 = ib.GetShort();
            //int contentI = ib.GetInt();
            //string contentS = ib.GetString();
            //Debug.Log("head:"+head+" len0:"+len0+" cmd0:"+cmd0+" contentI:"+contentI+" contentS:"+contentS);

            Send(bs);

        }

        private bool Send(byte[] buff)
        {
            if (!BeConnect())
            {
                Debug.LogError("SocketClient.Send 网络连接断开，发送消息失败");

                return false;
            }
            lock (this)
            {
                try
                {

                    if (BeMsgSending)
                    {
                        SendMsgQueue.Enqueue(buff);
                    }
                    else
                    {
                        BeMsgSending = true;
                        try
                        {
                            sendMsgLen += buff.Length;
                            //Debug.LogError ("已发送数据：" + sendMsgLen / 1024 + "kb，合计：" + (RevMsgTotalSize + sendMsgLen) / 1024);
                            _NetworkStream.BeginWrite(buff, 0, buff.Length, new AsyncCallback(OnSend), null);
                        }
                        catch (Exception ex)
                        {
                            //MemoryStream ts = new MemoryStream(buff);
                            //BinaryReader br = new BinaryReader(ts, Encoding.UTF8);
                            //BinaryHelper.ReadShort(br);
                            //BinaryHelper.ReadShort(br);
                            //int function_id = BinaryHelper.ReadInt(br);
                            //string msg = ">>>send exception, msg id:";
                            //msg += function_id;
                            //msg += ";ex: ";
                            //msg += ex.Message;
                            Debug.LogError("SocketClien.Send exception:" + ex.Message);
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;

        }

        private void OnSend(IAsyncResult ar)
        {
            lock (this)
            {
                try
                {
                    _NetworkStream.EndWrite(ar);
                    if (SendMsgQueue.Count > 0)
                    {
                        byte[] bData = SendMsgQueue.Dequeue();
                        sendMsgLen += bData.Length;
                        //Debug.Log("已发送数据：" + sendMsgLen / 1024 + "kb，合计：" + (RevMsgTotalSize + sendMsgLen) / 1024);
                        _NetworkStream.BeginWrite(bData, 0, bData.Length, new AsyncCallback(OnSend), null);
                    }
                    else
                    {
                        BeMsgSending = false;
                    }
                }
                catch (Exception ex)
                {
                    //14-01-09 jmn 掉线严重，去掉close，验证，添加catch信息
                    //添加上传服务器信息					
                    //string msg = ">>>Onsend exception:";
                    //msg += ex.Message;
                    ////####
                    //User32API.BugReport(msg, "jmn");
                    //TTrace.p(">>>Onsend exception:", msg);

                    Debug.LogError("SocketClien.OnSend exception:" + ex.Message);
                }
            }
        }

        #endregion

        #region rec

        private SocketPackMgr _SocketDataMgr;

        public static Queue<Cache4RecMsg> MsgQue = new Queue<Cache4RecMsg>();

        private void OnProcessOneMsg(Cache4RecMsg msg)
        {
            MsgQue.Enqueue(msg);
        }

        //注册接收消息
        public static Dictionary<string, Dictionary<short, Func<Cmd4Rec>>> RecMsgDic = new Dictionary<string, Dictionary<short, Func<Cmd4Rec>>>();

        public static void RegistRecMsg(string modelName, short cmd, Func<Cmd4Rec> recCmd)
        {
            if (!RecMsgDic.ContainsKey(modelName))
            {
                RecMsgDic.Add(modelName, new Dictionary<short, Func<Cmd4Rec>>());
            }

            if (!RecMsgDic[modelName].ContainsKey(cmd))
            {
                RecMsgDic[modelName].Add(cmd, recCmd);
            }
            else
            {
                Debug.LogError("ClientSocketMgr.RegistRecMsg faile 键值已存在，多次注册 modelName:"+ modelName+" cmd:"+cmd);
            }

        }

        #endregion


    }

}