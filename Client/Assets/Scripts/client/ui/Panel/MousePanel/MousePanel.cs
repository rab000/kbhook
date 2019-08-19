using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MousePanel : BasePanel
{
    public static MousePanel Ins;   

    protected override void Awake()
    {
        base.Awake();

        PanelName = "mousePanel";

        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    
}
