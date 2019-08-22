using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPanel : BasePanel
{
    public static NormalPanel Ins;

    protected override void Awake()
    {
        
        base.Awake();

        PanelName = "normalPanel";

        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    public override void ProcessPressAbleKeyUI(string key, bool bPress)
    {
        switch (key)
        {
            case "alt":
                break;
            case "shift":
                break;
            case "ctrl":
                break;
            case "caps":
                break;
        }
    }

}
