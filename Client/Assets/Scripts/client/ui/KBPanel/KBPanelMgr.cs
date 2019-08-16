using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBPanelMgr : MonoBehaviour
{

    public static bool BeCapsLock;

    public static bool BeShiftOn;

    public static bool BeAltOn;

    public static bool BeCtrlOn;

    void OnEnable()
    {
        SimpleEventMgr.Regsit(Default.EVENT_KB_CLICK, ProcessKBPress);
    }

    void OnDisable()
    {
        SimpleEventMgr.Remove(Default.EVENT_KB_CLICK);
    }

    void ProcessKBPress(string name,object data)
    {

        string key = (string)data;

        Debug.Log("KBPanel.ProcessKBPress->客户端按下:"+key);

        if (BeShiftOn)
        {
            BeShiftOn = false;
        }

        if (BeAltOn)
        {
            BeAltOn = false;
        }

        if (BeCtrlOn)
        {
            BeCtrlOn = false;
        }

        //NTODO 发消息


    }


}
