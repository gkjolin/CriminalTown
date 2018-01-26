﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Drop
{
    public static void DropObject<TCust>(out TCust customization)
    {
        GameObject draggedItem = Drag.ItemBeingDragged;
        Drag dragHandler = draggedItem.GetComponent<Drag>();

        draggedItem.GetComponent<Animator>().SetTrigger("Dropped");


        draggedItem.transform.SetParent(dragHandler.StartParent);
        draggedItem.transform.localPosition = dragHandler.StartPos;
        dragHandler.GetComponent<CanvasGroup>().blocksRaycasts = true;

        customization = dragHandler.StartParent.GetComponent<TCust>();
    }

    //public static void DropObject<TCust, TDrag>(out TCust customization, out TDrag itemDragHandler) where TDrag : Drag
    //{
    //    GameObject draggedItem = Drag.ItemBeingDragged;
    //    TDrag dragHandler = draggedItem.GetComponent<TDrag>();

    //    draggedItem.GetComponent<Animator>().SetTrigger("Dropped");


    //    draggedItem.transform.SetParent(dragHandler.StartParent);
    //    draggedItem.transform.localPosition = dragHandler.StartPos;
    //    dragHandler.GetComponent<CanvasGroup>().blocksRaycasts = true;

    //    customization = dragHandler.StartParent.GetComponent<TCust>();
    //    itemDragHandler = dragHandler;
    //}
}