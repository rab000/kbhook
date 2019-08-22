using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSubNumPanel : BasePanel
{
    public static FullSubNumPanel Ins;

    protected override void Awake()
    {
        base.Awake();

        Ins = this;

        PanelName = Default.FULL_SUB_PANEL_NUM;

    }
}
