using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Net.NetworkInformation;
namespace nsocket
{
    /// <summary>
    /// 网络数据包解析管理
    /// 处理拆包，粘包，半包,最终生成缓存消息
    /// </summary>
    public class SocketPackMgr
    {
        public Action<string> CloseEvent;

        public Action<Cache4RecMsg> OnProcessOnMsg;

        private const short HEAD_MSG = 127;

        /// <summary>
        /// netBuffer容量，一次最多能从网络流中读取的容量
        /// </summary>
        private const int NET_BUFFER_SIZE = 102400;

        private Stream _NetworkStream;

        public Socket _Socket;

        public SocketPackMgr(Socket socket, NetworkStream stream)
        {
            _Socket = socket;

            _NetworkStream = stream;

            RevMsgTotalSize = 0;
        }

        /// <summary>
        /// 记录下接收到的总byte长度
        /// </summary>
        private double RevMsgTotalSize = 0;
        /// <summary>
        /// 记录每次从Stream中读到NetBuffer的byte数
        /// </summary>
        private int BytesRecOnce;
        /// <summary>
        /// 当前数据NetBuffer(就是NetBuffer1或者NetBuffer2)中上一次数据处理没处理完的，剩余的byte数
        /// </summary>
        private int BytesNumWait4Process = 0;

        /// <summary>
        /// 从socket中读取(接收)一次数据(缓存到1级缓存NetBufer)
        /// 
        /// 注意
        /// 1 每次从socket接收的数据长度不等
        /// 2 本地一级缓存NetBuffer是固定大小(102400)
        /// 3 本地二级缓存NetBuffer1或NetBuffer2中可能包含有上一次没处理完的数据(半包)
        /// 4 最大读取量为102400，但如果上一次有没处理完的消息(半包)，那么读取最大长度为102400-上一次遗留的数据长度
        /// 如果socket中的数据大于这个值，那么下次再读    
        /// 
        /// </summary>
        public void ReadOnceFromSocket()
        {
            if (_Socket == null || !_Socket.Connected)
            {

                CloseEvent?.Invoke("ReadOnceFromSocket");

                return;
            }

            try
            {
                int netBufferLength = NetBuffer.Length;
                Array.Clear(NetBuffer, 0, netBufferLength);
                if (netBufferLength > BytesNumWait4Process)
                {
                    //ninfo 这里能读到的最大数是netbuffer.Length，之所以写netbuffer.Length - refreadpos是怕读多了，超出缓冲能处理的范围
                    //ninfo stream从网络一次最大读取netbuffer.length，stream中存储的来自网络的量可能比这个length要大，但大多情况是比这个小
                    //ninfo 后面自所以处理很复杂，原因就在于stream.BeginRead时网络stream中不一定有多少数据
                    int len = netBufferLength - BytesNumWait4Process;
                    //_Socket.BeginReceive(NetBuffer, 0, len, SocketFlags.None,new AsyncCallback(ReadFromNetBuffer), null);
                    _NetworkStream.BeginRead(NetBuffer, 0, len, new AsyncCallback(ReadFromNetBuffer), null);
                }
                else
                {
                    Debug.LogError("SocketDataMgr.SetupRecieveCallback 数据流溢出");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("SocketDataMgr.SetupRecieveCallback 数据读取回掉异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 处理网络流缓冲
        /// 
        /// 输入1级缓存数据，填充二级缓存，并解析消息(ProcNetMessage中处理)
        /// 1 从1级缓存copy数据到2级缓存
        /// 2 由ProcNetMessage解析2级缓存CurNetBuffer中的数据
        /// 3 解析后剩余数据放入到2级缓存NextNetBuffer中
        /// </summary>
        /// <param name="ar"></param>
        private void ReadFromNetBuffer(IAsyncResult ar)
        {
            //Debug.Log("ReadFromNetBuffer------" );

            if (_Socket == null || !_Socket.Connected)
            {
                CloseEvent?.Invoke("OnRecievedData");
                return;
            }

            try
            {

                //ninfo 这句返回的是stream真实（向netbuffer中）读了多少数据
                BytesRecOnce = _NetworkStream.EndRead(ar);

                Debug.Log("BytesRecOnce------" + BytesRecOnce);

                if (BytesRecOnce > 0)
                {
                    if (BytesRecOnce > int.MaxValue)
                        Debug.Log("SocketPackMgr.OnRecievedData 消息太大了！！！nBytesRec=" + BytesRecOnce);

                    RevMsgTotalSize += BytesRecOnce;
                    //Debug.Log("TcpNetMgr OnRecievedData 已接收数据：" + RevMsgTotalSize / 1024 + "kb，合计：" + (RevMsgTotalSize + sendMsgLen) / 1024);

                    byte[] curbuffer = GetCurrentBuffer();

                    byte[] nextbuffer = GetNextBuffer();

                    Array.Clear(nextbuffer, 0, nextbuffer.Length);

                    int effectiveSizeInBuffer = BytesNumWait4Process + BytesRecOnce;

                    if (effectiveSizeInBuffer >= NET_BUFFER_SIZE)
                    {//清空，从头来
                        Debug.LogError("SocketPackMgr.OnRecievedData [严重bug]已经越界了！refreadpos=" + BytesNumWait4Process + " nBytesRec=" + BytesRecOnce + " nBufferCount=" + NET_BUFFER_SIZE);
                        BytesNumWait4Process = 0;
                        effectiveSizeInBuffer = BytesRecOnce;
                    }

                    if (effectiveSizeInBuffer < NET_BUFFER_SIZE)
                    {
                        //从1级缓冲NetBuffer向2级缓存CurNetBuffer copy 数据
                        for (int i = 0; i < BytesRecOnce; i++)
                        {
                            curbuffer[i + BytesNumWait4Process] = NetBuffer[i];
                        }

                        //清理可能存在的脏数据
                        for (int i = BytesNumWait4Process + BytesRecOnce; i < NetBuffer1.Length; i++)
                        {
                            curbuffer[i] = 0;
                        }

                        BytesNumWait4Process = 0;
                        
                        //ninfo 注意这里effectiveSizeInBuffer不是buf真实size，而是有效数据size
                        ProcNetMessage(curbuffer, effectiveSizeInBuffer);
                        
                        //2级缓存翻转，具体见NetBuffer1 NetBuffer2的说明
                        bFlipBuffer = !bFlipBuffer;

                    }
                    else
                    {
                        Debug.LogError("SocketPackMgr.OnRecievedData [严重bug]抛弃该消息，消息长度过长：" + BytesRecOnce + " nBufferCount=" + NET_BUFFER_SIZE);
                    }
                }
                else
                {
                    Debug.LogError("------------>receive faile");
                    //this.Closed("Receive");
                    //CloseEvent("Receive");
                    //Debug.LogError("SocketPackMgr.OnRecievedData recive failed!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("数据读取异常：" + ex.Message + " == " + ex.ToString());
            }
            //进行下一次读取
            ReadOnceFromSocket();
        }

        /// <summary>
        /// 从二级缓存CurNetBuffer解析数据
        /// 拆包，粘包，半包处理，消息头解析(消息头0x127,)
        /// 未处理完数据(半包，不完整的数据)，放到二级缓存NextNetBuffer中
        /// 
        /// 消息头结构按顺序说明
        /// short headMsg
        /// int msgLen  这个长度包含 cmd和消息本身，不包含 headMsg和msgLen
        /// short cmd
        /// 
        /// </summary>
        /// <param name="curNetBuffer"> </param>
        /// <param name=注意这里nEnd不是buf真实size，而是buf中有效数据size></param>
        private void ProcNetMessage(byte[] curNetBuffer, int effectByteNumInBuffer)
        {
            Debug.LogError("------>接受到有效位数:"+ effectByteNumInBuffer);

            int head_msgLen_size = 6;//head(short) + msgLen(int);

            MemoryStream tstream = new MemoryStream(curNetBuffer);

            BinaryReader _BinaryReader = new BinaryReader(tstream, Encoding.UTF8);

            int tempBinaryReaderPos = 0;


            while (_BinaryReader.BaseStream.Position < effectByteNumInBuffer)
            {

                //半包处理1: 剩余数据长度小于head+msgLen,直接存入二级缓存NextNetBuffer，下一次再解析
                long leftbytes = effectByteNumInBuffer - _BinaryReader.BaseStream.Position;
                if (leftbytes < head_msgLen_size)
                {

                    //如果小于4个字节，那么交给下个BUFFER
                    byte[] curbuffer = GetCurrentBuffer();
                    byte[] nextbuffer = GetNextBuffer();
                    for (int i = 0; i < leftbytes; i++)
                    {
                        nextbuffer[i] = curbuffer[_BinaryReader.BaseStream.Position + i];
                    }
                    BytesNumWait4Process = (int)leftbytes;

                    return;
                }

                short headmsg = BinaryHelper.ReadShort(_BinaryReader);

                if (HEAD_MSG != headmsg)
                {
                    Debug.LogError("SocketDataMgr.ProcNetMessage 解析消息头错误 headmsg:" + headmsg+" HEAD:"+ HEAD_MSG);

                    //NINFO 如果heamMsg错了，是否应该略过当前消息，处理下一条消息？
                    //下面方法有可能会找到假heamMsg，这个真假不好判断，
                    //所以直接return可能是更好的选择，不允许出现找不到headMsg的情况

                    //while (BinaryHelper.ReadShort(_BinaryReader) != TcpNetMgr.HEAD_MSG)
                    //{
                    //    ;
                    //}
                    //_BinaryReader.BaseStream.Position -= 2;
                    //tempBinaryReaderPos = (int)_BinaryReader.BaseStream.Position;
                    //continue;

                    return;
                }

                int msgLen = BinaryHelper.ReadInt(_BinaryReader);

                //半包处理2: 剩余数据长度小于msglen(一条消息的完整长度cmd+msg),
                //直接把headMsg +msgLen+剩余数据直接放到2级缓存NextNetBuffer中，等待下一次处理
                leftbytes = effectByteNumInBuffer - _BinaryReader.BaseStream.Position;
                if (leftbytes < msgLen)
                {

                    byte[] curbuffer = GetCurrentBuffer();
                    byte[] nextbuffer = GetNextBuffer();
                    for (int i = 0; i < leftbytes + head_msgLen_size; i++)
                    {
                        nextbuffer[i] = curbuffer[_BinaryReader.BaseStream.Position + i - head_msgLen_size];
                    }

                    BytesNumWait4Process = (int)leftbytes + head_msgLen_size;

                    return;
                }

                //开始缓存消息
                short cmd = BinaryHelper.ReadShort(_BinaryReader);

                try
                {
                    //NTODO 消息缓冲
                    //if (NetBackMgr.GetInst().NetBackAnalyzerDic.ContainsKey(cmd))
                    //{

                    //    Byte[] content = _BinaryReader.ReadBytes(len);

                    //    Cache4RecMsg recevieMessage = new Cache4RecMsg();

                    //    recevieMessage.cmd = cmd;
                    //    recevieMessage.protoLen = len;
                    //    recevieMessage.protoContent = content;

                    //    Log.e("TcpNetMgr", "ProcNetMessage", "消息id:" + cmd + " len:" + len + "进入NetBackMgr.Cache4RecMsgQueue" + " readerPos:" + _BinaryReader.BaseStream.Position, BeShowLog);

                    //    NetBackMgr.Cache4RecMsgQueue.Enqueue(recevieMessage);

                    //}
                    int len0 = msgLen - 2;
                    Byte[] content = _BinaryReader.ReadBytes(msgLen - 2);//2 cmd(short)
                    Cache4RecMsg msg = new Cache4RecMsg();
                    msg.SessionID = "";
                    msg.Cmd = cmd;
                    msg.Data = content;
                    OnProcessOnMsg?.Invoke(msg);

                }
                catch (Exception e)
                {
                    Debug.LogError("SocketDataMgr.ProcNetMessage 缓存消息出错 cmd:" + cmd);
                }
                finally
                {
                    tempBinaryReaderPos += (msgLen + 6);//head(2)+msgLen(4)
                    if (_BinaryReader.BaseStream.Position != tempBinaryReaderPos)
                    {
                        Debug.LogError("SocketDataMgr.ProcNetMessage 消息处理函数读取数据错误 cmd:" + cmd);
                        _BinaryReader.BaseStream.Position = tempBinaryReaderPos;
                    }
                }//finally结束
            }//while结束
        }//函数结尾

        #region netBuffer

        //控制2级缓存翻转
        private bool bFlipBuffer = true;

        /// <summary>
        /// 1级缓存
        /// 每次从(网络流)Stream中读取的流数据时
        /// 先对netbuffer清零
        /// 然后把读到的数据直接存到这个buffer中
        /// </summary>
        private byte[] NetBuffer = new byte[NET_BUFFER_SIZE];

        /// <summary>
        /// 二级缓存
        /// 
        /// 用于处理数据的双缓存
        /// 具体流程
        /// 1 从网络流中读取数据到NetBuffer
        /// 2 把NetBuffer中的数据转存到 处理数据的缓存CurNetBuffer(NetBuffer1)
        /// 3 处理CurNetBuffer中的数据，如果发现CurNetBuffer中不足一条数据，就把剩余数据放到NextNetBuffer(NetBuffer2)中
        /// 4 翻转CurNetBuffer与NextNetBuffer，就是切换NetBuffer1，NetBuffer2代表的含义
        /// 5 当下一次向数据缓存CurNetBuffer存数据时，因为CurNetBuffer(NetBuffer2)中还有一部分数据，
        /// 把新读入的数据放到上一次剩余数据的后面，然后在进行数据解析处理
        /// 
        /// 所以双缓存的意义在于，
        /// CurNetBuffer(里面可能已经包含上次没处理完的数据)用来接收NetBuffer中新读取的数据
        /// NextNetBuffer用来保存一次没处理完的数据
        /// 当一轮读取完成后，Cur与Next互换，Next成为Cur(里面可能会包含上次没处理完的数据)
        /// </summary>
        private byte[] NetBuffer1 = new byte[NET_BUFFER_SIZE];

        private byte[] NetBuffer2 = new byte[NET_BUFFER_SIZE];

        //获取用于接受1级缓存数据，并解析数据的2级缓存
        private byte[] GetCurrentBuffer() { return bFlipBuffer ? NetBuffer1 : NetBuffer2; }
        //获取用于保存一次解析剩余数据的2级缓存
        private byte[] GetNextBuffer() { return bFlipBuffer ? NetBuffer2 : NetBuffer1; }

        #endregion

    }

}