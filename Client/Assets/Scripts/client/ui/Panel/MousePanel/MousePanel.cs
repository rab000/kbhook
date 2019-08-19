using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MousePanel : BasePanel
{
    public static MousePanel Ins;

    public Button MouseLeftButton;

    public Button MouseRightButton;

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

    void OnEnable()
    {
        MouseLeftButton.onClick.AddListener(OnClickMouseLeft);

        MouseRightButton.onClick.AddListener(OnClickMouseRight);

        //SimpleEventMgr.Regsit(Default.eve)

    }

    void OnDisable()
    {
        MouseLeftButton.onClick.RemoveListener(OnClickMouseLeft);

        MouseRightButton.onClick.RemoveListener(OnClickMouseRight);
    }

    private void OnClickMouseLeft()
    {

    }

    private void OnClickMouseRight()
    {

    }

}
