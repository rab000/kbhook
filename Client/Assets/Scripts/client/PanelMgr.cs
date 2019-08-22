using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 面板主管理器
/// </summary>
public class PannelMgr : MonoBehaviour
{

    public static bool BeCapsLock;

    public static bool BeShiftOn;

    public static bool BeAltOn;

    public static bool BeCtrlOn;

    public BasePanel CurPanel;

    void Start()
    {      
        NormalPanel.Ins.gameObject.SetActive(false);
        CodePanel.Ins.gameObject.SetActive(false);
        NumPanel.Ins.gameObject.SetActive(false);
        FullPanel.Ins.gameObject.SetActive(false);
        ProcessSwitchPanel(Default.EVENT_SWITCH_PANEL, Default.PANEL_TYPE_FULL);
    }

    void OnEnable()
    {
       
        SimpleEventMgr.Regsit(Default.EVENT_KB_CLICK, ProcessKBPress);
        SimpleEventMgr.Regsit(Default.EVENT_MOUSE, ProcessMouse);
        SimpleEventMgr.Regsit(Default.EVENT_SWITCH_PANEL, ProcessSwitchPanel);
        
    }

    void OnDisable()
    {
        SimpleEventMgr.Remove(Default.EVENT_KB_CLICK);
        SimpleEventMgr.Remove(Default.EVENT_MOUSE);
        SimpleEventMgr.Remove(Default.EVENT_SWITCH_PANEL);
    }
   
    void ProcessKBPress(string name,object data)
    {

        string key = (string)data;

        Debug.Log("KBPanel.ProcessKBPress->客户端按下:"+key);

        if (BeShiftOn)
        {
            BeShiftOn = false;

            CurPanel?.ProcessPressAbleKeyUI("shift",BeShiftOn);
        }

        if (BeAltOn)
        {
            BeAltOn = false;
            CurPanel?.ProcessPressAbleKeyUI("alt", BeAltOn);
        }

        if (BeCtrlOn)
        {
            BeCtrlOn = false;
            CurPanel?.ProcessPressAbleKeyUI("ctrl", BeCtrlOn);
        }

        if (AsyncTCPClient.Ins.BeConected())
        {
            Debug.Log("PanelMgr.ProcessKBPress sendMsg key:"+key);
            AsyncTCPClient.Ins.AsynSend(key);
        }
        else
        {
            Debug.LogError("sen msg when unconnect!!!");
        }
        

    }

    void ProcessMouse(string name, object data)
    {
        string key = (string)data;

        //Debug.Log("KBPanel.ProcessMouse->客户端按下:" + key);

        AsyncTCPClient.Ins.AsynSend(key);

    }

    //切换面板事件
    void ProcessSwitchPanel(string name,object data)
    {
        string panelName = (string)data;

        //Debug.Log("ProcessSwitchPanel panelName:"+panelName);

        if (CurPanel != null)
        {
            string curPanelName = CurPanel.PanelName;

            if (curPanelName.Equals(panelName))
            {
                Debug.Log("新切换panel与当前panel相同，不切换");
                return;
            }
            else
            {
                Debug.Log("新切换panel与当前panel不同");
                CurPanel.gameObject.SetActive(false);
            }    
        }

        switch (panelName)
        {
            case Default.PANEL_TYPE_NORMAL:
                CurPanel = NormalPanel.Ins;
                
                break;
            case Default.PANEL_TYPE_CODE:
                CurPanel = CodePanel.Ins;
                break;
            case Default.PANEL_TYPE_NUM:
                CurPanel = NumPanel.Ins;
                break;
            case Default.PANEL_TYPE_FULL:
                CurPanel = FullPanel.Ins;
                break;
            default:
                Debug.LogError("切换面板失败 panelName:"+panelName);
                break;
        }

        if(null!=CurPanel)CurPanel.gameObject.SetActive(true);

    }


}
