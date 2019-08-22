using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullPanel : BasePanel
{
    public static FullPanel Ins;

    public Button MouseLeftBtn;

    public Button MouseRightBtn;

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
    }

    void OnDisable()
    {
        MouseLeftBtn.onClick.RemoveAllListeners();
        MouseRightBtn.onClick.RemoveAllListeners();
    }

    public void OnMouseLeftClick()
    {
        SimpleEventMgr.Send(Default.EVENT_MOUSE, "mouse|left|0");
    }

    public void OnMouseRightClick()
    {
        SimpleEventMgr.Send(Default.EVENT_MOUSE, "mouse|right|0");
    }

}
