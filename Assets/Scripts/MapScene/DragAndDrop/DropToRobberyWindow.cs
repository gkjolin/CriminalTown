﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace CriminalTown {

    public class DropToRobberyWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        bool isEntered;


        public void OnPointerEnter(PointerEventData eventData) {
            isEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            isEntered = false;
        }

        //void Update()
        public void DropToRobberyWindowUpdate() {
            if (Input.GetMouseButtonUp(0)) {
                if (isEntered) {
                    int locNum = RobberyWindow.robberyData.LocationNum;
                    RobberyType robType = RobberyWindow.robberyData.RobberyType;
                    if (Drag.ItemType == DraggeableItemType.Character) {
                        CharacterCustomization charCust;
                        Drop.DropObject(out charCust);

                        if (charCust != null)
                            if (charCust.Character.Status == CharacterStatus.Normal)
                                UIManager.robberyWindow.TryToAddCharacterToRobbery(charCust.Character, robType, locNum);
                    }
                    if (Drag.ItemType == DraggeableItemType.Item) {
                        ItemCustomization iCust;
                        Drop.DropObject(out iCust);
                        if (iCust != null)
                            if (Drag.Location == DraggableObjectsLocations.itemsPanel)
                                UIManager.robberyWindow.TryToAddItemToRobbery(iCust.number, robType, locNum);
                    }
                    isEntered = false;
                } else //if pointer exited
                {
                    if (Drag.ItemType == DraggeableItemType.Character) {
                        CharacterCustomization charCust;
                        Drop.DropObject(out charCust);

                        if (charCust.Character.Status == CharacterStatus.Robbery)
                            if (Drag.Location == DraggableObjectsLocations.robbery)
                                RobberyWindow.robberyData.RemoveCharacter(charCust.Character);
                    }
                    if (Drag.ItemType == DraggeableItemType.Item) {
                        ItemCustomization iCust;
                        Drop.DropObject(out iCust);

                        if (Drag.Location == DraggableObjectsLocations.robbery)
                            UIManager.robberyWindow.TryToRemoveItemFromRobbery(iCust.number, RobberyWindow.robberyData.RobberyType, RobberyWindow.robberyData.LocationNum);
                    }
                    isEntered = false;
                }
            }
        }
    }

}