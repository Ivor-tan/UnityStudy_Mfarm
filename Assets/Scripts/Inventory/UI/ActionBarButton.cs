using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory {

    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode keyCode;
        private SlotUI slotUI;
        public bool canUse;

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }
        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }



        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(keyCode) && canUse)
            {
                if (slotUI.itemDetails!=null)
                {
                    slotUI.isSelected = !slotUI.isSelected;
                    if (slotUI.isSelected)
                    {
                        slotUI.inventoryUI.UpdateHightlight(slotUI.slotIndex);
                    }
                    else
                    {
                        slotUI.inventoryUI.UpdateHightlight(-1);
                    }

                    EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
                }
            }
        }

        private void OnUpdateGameStateEvent(GameState gameState)
        {
            canUse = (gameState == GameState.GamePlay);
        }
    }
}

