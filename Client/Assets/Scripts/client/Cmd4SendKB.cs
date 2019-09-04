using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cmd4SendKB : Cmd4Send
{

    public string key;

    public Cmd4SendKB()
    {
        this.Cmd = Default.CMD_KB;
    }

    public override void EncodeContent(IoBuffer buffer)
    {
        buffer.PutString(key);
    }

}
