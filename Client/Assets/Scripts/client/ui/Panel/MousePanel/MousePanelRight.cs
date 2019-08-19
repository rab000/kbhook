using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePanelRight : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        SimpleEventMgr.Send(Default.EVENT_MOUSE, "mouse|right|0");
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

}
