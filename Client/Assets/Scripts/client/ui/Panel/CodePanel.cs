using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodePanel : BasePanel
{
    public static CodePanel Ins;

    protected override void Awake()
    {

        base.Awake();

        PanelName = Default.PANEL_TYPE_CODE;

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
