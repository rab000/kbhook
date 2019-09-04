using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cmd4RecKB : Cmd4Rec
{
    public Cmd4RecKB()
    {
        this.Cmd = Default.CMD_KB;
    }

    public override void Decode(byte[] bs)
    {
        Debug.Log("Cmd4RecKB.Decode");

        IoBuffer buffer = new IoBuffer(1024);

        buffer.PutBytes(bs);

        string key = buffer.GetString();

        ServerMgr.Ins.ProcessKB(key);
    }

    public override void Excute()
    {
        Debug.Log("服务端excute kb");
    }
}
