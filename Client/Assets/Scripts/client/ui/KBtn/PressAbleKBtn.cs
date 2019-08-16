using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PressAbleKBtn : BaseKBtn
{    
    //是否已经被按下
    public bool BePressDown = false;

    private Image SelfImg;

    protected override void Awake()
    {
        base.Awake();

        Type = KBtnTypeEnum.pressAble;

        SelfImg = gameObject.GetComponent<Image>();

    }

    protected override void OnClick()
    {
        BePressDown = !BePressDown;

        if (BePressDown)
        {
            SelfImg.color = Color.gray;
        }
        else
        {
            SelfImg.color = Color.white;
        }

    }

}
