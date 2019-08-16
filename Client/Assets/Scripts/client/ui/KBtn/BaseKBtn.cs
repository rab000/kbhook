using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseKBtn : MonoBehaviour
{

    Button SelfBtn;

    public string Name;

    protected KBtnTypeEnum Type;

    protected virtual void Awake()
    {
        SelfBtn = gameObject.GetComponent<Button>();
    }

    void OnEnable()
    {
        SelfBtn.onClick.AddListener(Click);
    }

    void OnDisable()
    {
        SelfBtn.onClick.RemoveListener(Click);
    }

    void Click()
    {
        OnClick();
    }

    protected virtual void OnClick()
    {
       
    }
}
