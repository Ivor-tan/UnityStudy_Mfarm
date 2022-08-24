using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory {
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public Text itemName;
        public InputField tradeAmount;
        public Button submitButton;
        public Button cancelButton;

        private ItemDetails item;
        private bool isSellTrade;

        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);
            submitButton.onClick.AddListener(TradeItem);
        }


        public void SetupTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;
            isSellTrade = isSell;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            tradeAmount.text = string.Empty;

        }

        private void TradeItem()
        {
            int amount = Convert.ToInt32(tradeAmount.text);

            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);

            CancelTrade();
        }

        public void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}

