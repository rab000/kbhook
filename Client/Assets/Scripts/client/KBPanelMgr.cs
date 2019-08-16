using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 面板主管理器
/// </summary>
public class KBPanelMgr : MonoBehaviour
{

    public static bool BeCapsLock;

    public static bool BeShiftOn;

    public static bool BeAltOn;

    public static bool BeCtrlOn;

    public BasePanel CurPanel;

    

    void OnEnable()
    {
        SimpleEventMgr.Regsit(Default.EVENT_KB_CLICK, ProcessKBPress);
        SimpleEventMgr.Regsit(Default.EVENT_SWITCH_PANEL, ProcessSwitchPanel);
    }

    void OnDisable()
    {
        SimpleEventMgr.Remove(Default.EVENT_KB_CLICK);
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
            Debug.Log("KBPanelMgr.ProcessKBPress sendMsg key:"+key);
            AsyncTCPClient.Ins.AsynSend(key);
        }
        else
        {
            Debug.LogError("sen msg when unconnect!!!");
        }
        

    }

    //切换面板事件
    void ProcessSwitchPanel(string name,object data)
    {
        string panelName = (string)data;

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
                CurPanel.gameObject.SetActive(false);
            }    
        }

        switch (panelName)
        {
            case "normalPanel":
                CurPanel = NormalPanel.Ins;
                
                break;
            case "codePanel":
                CurPanel = CodePanel.Ins;
                break;
            case "numPanel":
                CurPanel = NumPanel.Ins;
                break;
            case "mousePanel":
                CurPanel = MousePanel.Ins;
                break;
            default:
                Debug.LogError("切换面板失败 panelName:"+panelName);
                break;
        }

        if(null!=CurPanel)CurPanel.gameObject.SetActive(true);

    }
}
