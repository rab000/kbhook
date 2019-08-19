using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumPanel : BasePanel
{
    public static NumPanel Ins;

    protected override void Awake()
    {
        base.Awake();

        PanelName = "numPanel";

        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    void Start()
    {
        
    }
   
    void Update()
    {
        
    }
}
