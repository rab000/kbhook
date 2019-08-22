using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePanelMidArea : MonoBehaviour,IDragHandler
{    

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("mid deta:"+eventData.delta.y);
        SendMouseMidEvent((int)eventData.delta.y);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    private void SendMouseMidEvent(int deltaY)
    {
        Default.SB.Clear();
        Default.SB.Append("mouse|mid|");
        float offy = deltaY * Default.MOUSE_MID_SPEED;
        Default.SB.Append((int)offy);
        string key = Default.SB.ToString();
        SimpleEventMgr.Send(Default.EVENT_MOUSE, key);
    }

}
