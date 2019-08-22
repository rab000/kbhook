using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullPanelKBMgr : MonoBehaviour
{

    private BasePanel CurPanel;

    public FullSubKBPanel _FullSubKBPanel;

    public FullSubNumPanel _FullSubNumPanel;

    public FullSubCodePanel _FullSubCodePanel;

    public FullSubVSPanel _FullSubVSPanel;

    public FullSubUnityPanel _FullSubUnityPanel;

    void Start()
    {
        _FullSubKBPanel.gameObject.SetActive(false);
        _FullSubNumPanel.gameObject.SetActive(false);
        _FullSubCodePanel.gameObject.SetActive(false);
        _FullSubVSPanel.gameObject.SetActive(false);
        _FullSubUnityPanel.gameObject.SetActive(false);
        SwitchFullSubPanel(Default.EVENT_SWITCH_FULL_PANEL, Default.FULL_SUB_PANEL_KB);
    }

    void OnEnable()
    {
       
        SimpleEventMgr.Regsit(Default.EVENT_SWITCH_FULL_PANEL, SwitchFullSubPanel);
    }

    void OnDisable()
    {
        
        SimpleEventMgr.Remove(Default.EVENT_SWITCH_FULL_PANEL);
    }

    private void SwitchFullSubPanel(string name, object tagetSubPanelName)
    {
        string subPanelName = (string)tagetSubPanelName;

        if (CurPanel != null)
        {
            string curPanelName = CurPanel.PanelName;

            if (curPanelName.Equals(subPanelName))
            {
                Debug.Log("FullPanel.SwitchFullSubPanel 新切换panel与当前panel相同，不切换");
                return;
            }
            else
            {
                Debug.Log("ullPanel.SwitchFullSubPanel 新切换panel与当前panel不同");
                CurPanel.gameObject.SetActive(false);
            }
        }


        switch (subPanelName)
        {
            case Default.FULL_SUB_PANEL_KB:
                CurPanel = _FullSubKBPanel;
                break;
            case Default.FULL_SUB_PANEL_NUM:
                CurPanel = _FullSubNumPanel;
                break;
            case Default.FULL_SUB_PANEL_CODE:
                CurPanel = _FullSubCodePanel;
                break;
            case Default.FULL_SUB_PANEL_UNITY:
                CurPanel = _FullSubUnityPanel;
                break;
            case Default.FULL_SUB_PANEL_VS:
                CurPanel = _FullSubVSPanel;
                break;
        }

        if (null != CurPanel) CurPanel.gameObject.SetActive(true);
    }
}
