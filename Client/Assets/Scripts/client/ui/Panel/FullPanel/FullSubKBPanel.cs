using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSubKBPanel : BasePanel
{
    public static FullSubKBPanel Ins;

    protected override void Awake()
    {
        base.Awake();

        Ins = this;

        PanelName = Default.FULL_SUB_PANEL_KB;

    }
}
