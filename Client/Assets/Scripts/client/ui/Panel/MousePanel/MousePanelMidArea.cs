using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePanelMidArea : MonoBehaviour,IDragHandler
{    

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("deta:"+eventData.delta);
        SendMouseMidEvent(eventData.delta.y);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    private void SendMouseMidEvent(float deltaY)
    {
        Default.SB.Clear();
        Default.SB.Append("mouse|mid|");
        int offy = deltaY * Default.MOUSE_MID_SPEED;
        Default.SB.Append(offy);
        string key = Default.SB.ToString();
        SimpleEventMgr.Send(Default.EVENT_MOUSE, key);
    }

}
