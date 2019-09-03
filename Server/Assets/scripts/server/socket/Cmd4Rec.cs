
public class Cmd4Rec 
{
    public short Cmd;

    public void Process(byte[] bs)
    {
        Decode(bs);
        Excute();
    }

    /// <summary>
    /// 解析数据
    /// </summary>
    public virtual void Decode(byte[] bs)
    {

    }

    /// <summary>
    /// 执行消息内容
    /// </summary>
    public virtual void Excute()
    {

    }

}
