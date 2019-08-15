using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharKBtn : BaseKBtn
{
    protected override void Awake()
    {
        base.Awake();

        Type = KBtnTypeEnum.char0;
    }

    protected override void OnClick()
    {
        if (KBPanelMgr.BeCapsLock)
        {
            SimpleEventMgr.Send(Default.EVENT_KB_CLICK, Name.ToUpper());
        }
        else
        {
            SimpleEventMgr.Send(Default.EVENT_KB_CLICK, Name.ToLower());
        }

    }

}
