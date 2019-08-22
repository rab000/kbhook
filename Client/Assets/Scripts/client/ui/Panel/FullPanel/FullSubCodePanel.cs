using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSubCodePanel : BasePanel
{
    public static FullSubCodePanel Ins;

    protected override void Awake()
    {
        base.Awake();

        Ins = this;

        PanelName = Default.FULL_SUB_PANEL_CODE;

    }

    public override void ProcessPressAbleKeyUI(string key, bool bPress)
    {
        base.ProcessPressAbleKeyUI(key,bPress);
    }
}
