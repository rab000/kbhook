using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cmd4SendMouse : Cmd4Send
{
    public string mouseData;

    public Cmd4SendMouse()
    {
        this.Cmd = Default.CMD_MOUSE;
    }

    public override void EncodeContent(IoBuffer buffer)
    {
        buffer.PutString(mouseData);
    }

}
