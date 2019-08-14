﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NTest : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            SocketServer.Ins.StartServer(tempCallback);
            Debug.Log("start server!");
        }

        if (Input.GetKeyUp(KeyCode.F2))
        {
            SocketClient.Ins.SetIP("127.0.0.1", 54321);

            SocketClient.Ins.StartConnect();

            //Client.Ins.SetState(Client.ClientStateEnum.Keyboard);

            Debug.Log("start client!");
        }

        if (Input.GetKeyUp(KeyCode.F3))
        {
            SocketServer.Ins.SafeClose();
            Debug.Log("close server!");
        }

        if (Input.GetKeyUp(KeyCode.F4))
        {
            SocketClient.Ins.SafeClose();

            Debug.Log("stop client!");
        }

        if (Input.GetKeyUp(KeyCode.F5))
        {
            SocketClient.Ins.SafeClose();

            IoBuffer ib = new IoBuffer();

            ib.PutString("test");

            SocketClient.Ins.SendMsg(ib.ToArray());

            Debug.Log("client send msg!");
        }

    }

    private void tempCallback(byte[] bs)
    {

        Debug.Log("rec bs.len:"+bs.Length);

        IoBuffer ib = new IoBuffer();

        ib.PutBytes(bs);

        var s =ib.GetString();

        Debug.Log("server rec msg:"+s);
    }
}
