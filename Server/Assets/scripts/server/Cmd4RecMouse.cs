using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cmd4RecMouse : Cmd4Rec
{
    public Cmd4RecMouse()
    {
        this.Cmd = Default.CMD_MOUSE;
    }

    public override void Decode(byte[] bs)
    {
        Debug.Log("Cmd4RecMouse.Decode");

        IoBuffer buffer = new IoBuffer(1024);

        buffer.PutBytes(bs);

        string key = buffer.GetString();

        ServerMgr.Ins.ProcessMouse(key);
    }

    public override void Excute()
    {
        Debug.Log("服务端excute Mouse");
    }
}
