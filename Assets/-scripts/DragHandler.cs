using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, ISelectHandler, IDeselectHandler
{
    private Vector2 dragStart;

    public InterfaceManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<InterfaceManager>();
    }

    public void Clicked()
    {
        uiManager.ClickedNode(transform.parent.gameObject);
    }
    
    #region IBeginDragHandler implementation

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStart = eventData.position;
    }

    #endregion
    
    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData)
    {
        transform.parent.localPosition += (Vector3) (eventData.position - dragStart);
        dragStart = eventData.position;
        
        if (uiManager.data[Int32.Parse(transform.parent.name)].linkedLinkCounter != 0)
            //foreach (GameObject gm in uiManager.data[Int32.Parse(transform.parent.name)].linkedLinks)
            for(int i = 0; i < uiManager.data[Int32.Parse(transform.parent.name)].linkedLinkCounter; i++)
            {
                uiManager.MoveLink(uiManager.GetLinkID(uiManager.data[Int32.Parse(transform.parent.name)].linkedLinks[i].name));
                //uiManager.MoveLink(uiManager.GetLinkID(gm.name));
            }
    }

    #endregion

    #region ISelectHandler implementation

    public void OnSelect(BaseEventData eventData)
    {
        uiManager.SelectNode(transform.parent.gameObject);
    }

    #endregion
    
    #region IDeselectHandler implementation

    public void OnDeselect(BaseEventData eventData)
    {
        uiManager.DeselectNode(transform.parent.gameObject);
    }

    #endregion
}
