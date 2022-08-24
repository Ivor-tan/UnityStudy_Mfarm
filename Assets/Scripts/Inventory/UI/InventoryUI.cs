using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {

        public ItemTooltip itemTooltip;

        [Header("拖拽图片")]
        public Image dragItem;

        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;

        [Header("通用背包UI")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;
        public GameObject boxSlotPrefab;
        [SerializeField] private List<SlotUI> baseBagSlots;

        [Header("交易UI")]
        public TradeUI tradeUI;
        public TextMeshProUGUI palyerMoney;

        [SerializeField] private SlotUI[] playerSlots;



        private void Start()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;
            palyerMoney.text = InventoryManager.Instance.playerMoney.ToString();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }
        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShwoTradeUI += OnShwoTradeUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShwoTradeUI -= OnShwoTradeUI;
        }

        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {

            switch (location)
            {
                case InventoryLocation.Player:

                    for (int i = 0; i < playerSlots.Length; i++)
                    {

                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);

                            playerSlots[i].UpDateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpDateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                case InventoryLocation.Shop:

                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {

                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);

                            baseBagSlots[i].UpDateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpDateEmptySlot();
                        }
                    }
                    break;
                default:
                    break;
            }
            palyerMoney.text = InventoryManager.Instance.playerMoney.ToString();
        }

        //打开背包UI
        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }

        //更新高亮
        public void UpdateHightlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHightlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHightlight.gameObject.SetActive(false);
                }
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            UpdateHightlight(-1);
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
       
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                SlotType.Box => boxSlotPrefab,
                _ => null,
                //SlotType.Box:
                //SlotType.Shop:
            };
            baseBag.SetActive(true);

            baseBagSlots = new List<SlotUI>();
        
            for (int i = 0; i < bagData.items.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }

            //Debug.Log("OnBaseBagOpenEvent ===================>" + baseBagSlots.Count);

            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());

            if (slotType ==SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-0.75f, 0.5f);
                bagUI.SetActive(true);
                bagOpened = true;
            }

            OnUpdateInventoryUI(InventoryLocation.Box, bagData.items);
        }

        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemTooltip.gameObject.SetActive(false);
            UpdateHightlight(-1);

            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                bagOpened = false;
            }
        }

        private void OnShwoTradeUI(ItemDetails itemDetails, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(itemDetails, isSell);
        }

    }

}

