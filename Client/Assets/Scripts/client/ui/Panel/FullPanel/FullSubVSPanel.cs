using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSubVSPanel : BasePanel
{
    public static FullSubVSPanel Ins;

    protected override void Awake()
    {
        base.Awake();

        Ins = this;

        PanelName = Default.FULL_SUB_PANEL_VS;

    }
    
}
