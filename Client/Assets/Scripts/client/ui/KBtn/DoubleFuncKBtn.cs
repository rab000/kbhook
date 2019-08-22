using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleFuncKBtn : BaseKBtn
{
    //可切换功能
    public string Char1;

    public string Char2;

    protected override void Awake()
    {
        base.Awake();

        Type = KBtnTypeEnum.doublefunc;
    }

    protected override void OnClick()
    {

        if (PanelMgr.BeShiftOn)
        {
            SimpleEventMgr.Send(Default.EVENT_KB_CLICK,Char2);
        }
        else
        {
            SimpleEventMgr.Send(Default.EVENT_KB_CLICK, Char1);
        }

    }

}
