using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MFarm.Inventory
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxTemplate;
        public InventoryBag_SO boxData;

        public GameObject mouseIcon;
        private bool canOpen =false;
        private bool isOpen;

        public int index;

        //场景原有的箱子数据不会保存，新建clone名字无法对应
        public bool isPlayBuild = false;

        private void Start()
        {
            if (!isPlayBuild)
            {
                initBox(index);
            }
        }
        private void OnEnable()
        {
            if (boxData == null)
            {
                boxData = Instantiate(boxTemplate);
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isOpen && canOpen && Input.GetMouseButtonDown(1))
            {
                EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxData);
                isOpen = true;
            }

            if (!canOpen && isOpen)
            {
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxData);
                isOpen = false;
            }

            if ( isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxData);
                isOpen = false;
            }

        }

        public void initBox(int boxIndex)
        {
            isPlayBuild = true;
            index = boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null)
            {
                boxData.items = InventoryManager.Instance.GetBoxDataList(key);
            }
            else
            {
                InventoryManager.Instance.AddBoxDataDict(this);
            }
        }
    }
}
