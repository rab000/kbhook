using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullSubUnityPanel : BasePanel
{
    public static FullSubUnityPanel Ins;

    protected override void Awake()
    {
        base.Awake();

        Ins = this;

        PanelName = Default.FULL_SUB_PANEL_UNITY;

    }
}
