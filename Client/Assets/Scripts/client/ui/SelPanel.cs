using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelPanel : MonoBehaviour
{

    public Button MouseBtn;

    public Button KBBtn;

    public Button NumBtn;

    public Button CodeBtn;

    public static SelPanel Ins;

    void Awake()
    {
        Ins = this;
    }

    void OnDestroy()
    {
        Ins = null;
    }

    void OnEnable()
    {
        MouseBtn.onClick.AddListener(OnMouseBtnClick);
        KBBtn.onClick.AddListener(OnKBBtnClick);
        NumBtn.onClick.AddListener(OnNumBtnClick);
        CodeBtn.onClick.AddListener(OnCodeBtnClick);
    }

    void OnDisable()
    {
        MouseBtn.onClick.RemoveAllListeners();
        KBBtn.onClick.RemoveAllListeners();
        NumBtn.onClick.RemoveAllListeners();
        CodeBtn.onClick.RemoveAllListeners();
    }

    private void OnMouseBtnClick()
    {
        SimpleEventMgr.Send(Default.EVENT_SWITCH_PANEL, "mousePanel");
    }

    private void OnKBBtnClick()
    {
        SimpleEventMgr.Send(Default.EVENT_SWITCH_PANEL, "normalPanel");
    }

    private void OnNumBtnClick()
    {
        SimpleEventMgr.Send(Default.EVENT_SWITCH_PANEL, "numPanel");
    }

    private void OnCodeBtnClick()
    {
        SimpleEventMgr.Send(Default.EVENT_SWITCH_PANEL, "codePanel");
    }
   

}
