using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
