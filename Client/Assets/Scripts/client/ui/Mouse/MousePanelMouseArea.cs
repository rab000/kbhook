using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePanelMouseArea : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("--deta:"+eventData.delta);
        SendMouseMoveEvent(eventData.delta);

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    private void SendMouseMoveEvent(Vector2 delta)
    {
        Default.SB.Clear();
        Default.SB.Append("mouse|move|");
        Vector2 v2 = delta * Default.MOUSE_MOVE_SPEED;       
        Default.SB.Append(v2.x);
        Default.SB.Append("|");
        Default.SB.Append(v2.y);
        string key = Default.SB.ToString();
        SimpleEventMgr.Send(Default.EVENT_MOUSE, key);
    }
}
