using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullPanel : BasePanel
{
    public static FullPanel Ins;

    public Button MouseLeftBtn;

    public Button MouseRightBtn;

    private BasePanel CurPanel;

    void Start()
    {
        FullSubKBPanel.Ins.gameObject.SetActive(false);
        FullSubNumPanel.Ins.gameObject.SetActive(false);
        FullSubCodePanel.Ins.gameObject.SetActive(false);
        FullSubVSPanel.Ins.gameObject.SetActive(false);
        FullSubUnityPanel.Ins.gameObject.SetActive(false);
        SwitchFullSubPanel(Default.EVENT_SWITCH_FULL_PANEL, Default.FULL_SUB_PANEL_KB);
    }

    protected override void Awake()
    {
        base.Awake();

        PanelName = Default.PANEL_TYPE_FULL;

        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    void OnEnable()
    {
        MouseLeftBtn.onClick.AddListener(OnMouseLeftClick);
        MouseRightBtn.onClick.AddListener(OnMouseRightClick);
        SimpleEventMgr.Regsit(Default.EVENT_SWITCH_FULL_PANEL, SwitchFullSubPanel);
    }

    void OnDisable()
    {
        MouseLeftBtn.onClick.RemoveAllListeners();
        MouseRightBtn.onClick.RemoveAllListeners();
        SimpleEventMgr.Remove(Default.EVENT_SWITCH_FULL_PANEL);
    }

    public void OnMouseLeftClick()
    {
        SimpleEventMgr.Send(Default.EVENT_MOUSE, "mouse|left|0");
    }

    public void OnMouseRightClick()
    {
        SimpleEventMgr.Send(Default.EVENT_MOUSE, "mouse|right|0");
    }

    private void SwitchFullSubPanel(string name,object tagetSubPanelName)
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
                CurPanel = FullSubKBPanel.Ins;
                break;
            case Default.FULL_SUB_PANEL_NUM:
                CurPanel = FullSubNumPanel.Ins;
                break;
            case Default.FULL_SUB_PANEL_CODE:
                CurPanel = FullSubCodePanel.Ins;
                break;
            case Default.FULL_SUB_PANEL_UNITY:
                CurPanel = FullSubUnityPanel.Ins;
                break;
            case Default.FULL_SUB_PANEL_VS:
                CurPanel = FullSubVSPanel.Ins;
                break;
        }

        if (null != CurPanel) CurPanel.gameObject.SetActive(true);
    }

}
