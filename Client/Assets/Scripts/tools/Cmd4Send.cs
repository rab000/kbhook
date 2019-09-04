

public class Cmd4Send
{
    public short Cmd;

    public static IoBuffer SendBuffer = new IoBuffer(10240);

    public byte[] Encode()
    {
        //head,len,cmd,content,这里只使用EncodeContent存具体内容，head，len，cmd都不在这里处理
        SendBuffer.Clear();
        //SendBuffer.PutShort(Cmd);
        EncodeContent(SendBuffer);
        return SendBuffer.ToArray();
    }

    public virtual void EncodeContent(IoBuffer buffer)
    {

    }

}
