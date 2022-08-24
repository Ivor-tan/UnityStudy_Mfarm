using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>,ISaveable
    {
        [Header("��Ʒ����")]
        public ItemDataList_SO itemDataList_SO;

        [Header("��ͼ����")]
        public BluePrintData_SO bluePrintData_SO;

        [Header("��������")]
        public InventoryBag_SO playerBag;

        public InventoryBag_SO playerBagTemp;

        private InventoryBag_SO currentBoxBag;

        [Header("�������")]
        public int playerMoney = 300;

        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();
        public int BoxDataAmount => boxDataDict.Count;

        public string guid => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HavrestAtPlayerPosition += OnHavrestAtPlayerPosition;
            EventHandler.BuildFurntureEvent += OnBuildFurntureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HavrestAtPlayerPosition -= OnHavrestAtPlayerPosition;
            EventHandler.BuildFurntureEvent -= OnBuildFurntureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }

        private void Start()
        {
            ((ISaveable)this).RegisterSaveable();
            //EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.items);
        }

        //��ȡ��Ʒ��Ϣ
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        //�����Ʒ������
        public void AddItem(Item item ,bool toDestory)
        {
            //������λ���Ƿ��и���Ʒ
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            if (toDestory)
            {
                Destroy(item.gameObject);
            }
            //����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.items);
        }

        //������λ
        private bool CheckBagCapacity()
        {

            for(int i =0;i< playerBag.items.Count; i++)
            {
                if(playerBag.items[i].itemID == 0)
                {
                    return true;
                }
            }
            return false;
        }

        //�Ƿ��и���Ʒ
        private int GetItemIndexInBag(int ID)
        {

            for (int i = 0; i < playerBag.items.Count; i++)
            {
                if (playerBag.items[i].itemID == ID)
                {
                    return i;
                }
            }
            return -1;
        }

        //ָ��λ�������Ʒ
        private void AddItemAtIndex(int ID,int index ,int amount)
        {
            if(index == -1 && CheckBagCapacity())
            {
                var item = new InventoryItem() { itemID =ID,itemAmount =amount};
                for (int i = 0; i < playerBag.items.Count; i++)
                {
                    if (playerBag.items[i].itemID == 0)
                    {
                        playerBag.items[i] = item;
                        break;
                    }
                }
            }
            else
            {
                int currentAmount = playerBag.items[index].itemAmount + amount;
                var item = new InventoryItem() { itemID = ID, itemAmount = currentAmount };
                playerBag.items[index] = item;
            }
        }

        //������Ʒ
        public void SwapItem(int fromIndex,int targetIndex)
        {
            InventoryItem currentItem = playerBag.items[fromIndex];
            InventoryItem targetItem = playerBag.items[targetIndex];

            if (targetItem.itemID !=0)
            {
                playerBag.items[fromIndex] = targetItem;
                playerBag.items[targetIndex] = currentItem;
            }
            else
            {
                playerBag.items[targetIndex] = currentItem;
                playerBag.items[fromIndex] = new InventoryItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.items);
        }

        public void SwapItem(InventoryLocation locationFrom,int fromIndex, InventoryLocation locationTarget, int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(locationTarget);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)  //�в���ͬ��������Ʒ
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //��ͬ��������Ʒ
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else    //Ŀ��ո���
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }

        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.items,
                InventoryLocation.Box => currentBoxBag.items,
                _ => null
            };
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }

        /// <summary>
        /// �������������ֵ�
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))
                boxDataDict.Add(key, box.boxData.items);
            //Debug.Log(key);
        }

        /// <summary>
        /// ��  ��ұ���  ���Ƴ�ָ��������Ʒ
        /// </summary>
        /// <param name="ID">��Ʒid</param>
        /// <param name="removeAmount">����</param>
        private void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);
            if (playerBag.items[index].itemAmount > removeAmount)
            {
                var amount = playerBag.items[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.items[index] = item;
            }
            else if (playerBag.items[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem ();
                playerBag.items[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.items);
        }

        private void OnStartNewGameEvent(int index)
        {
            playerBag = Instantiate(playerBagTemp);
            playerMoney = Settings.playerMoney;
            boxDataDict.Clear();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.items);
        }

        private void OnDropItemEvent(int itemId, Vector3 pos,ItemType itemType)
        {
            //�Ƴ�������Ʒ
            RemoveItem(itemId, 1);

        }

        private void OnHavrestAtPlayerPosition(int id)
        {
            var index = GetItemIndexInBag(id);

            AddItemAtIndex(id, index, 1);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.items);
        }

        private void OnBuildFurntureEvent(int id, Vector3 Pos)
        {
            RemoveItem(id, 1);
            BluePrintDetails bluePrint = bluePrintData_SO.GetBluePrintDetails(id);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }

        /// <summary>
        /// �򿪱���ʱ��ȡbox����
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bag_SO"></param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag = bag_SO;
        }

        /// <summary>
        /// ������Ʒ
        /// </summary>
        /// <param name="itemDetails">��Ʒ��Ϣ</param>
        /// <param name="amount">��������</param>
        /// <param name="isSellTrade">�Ƿ�������</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //�����Ʒ����λ��
            int index = GetItemIndexInBag(itemDetails.itemID);

            if (isSellTrade)    //��
            {
                if (playerBag.items[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    //�����ܼ�
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney - cost >= 0)   //��
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;
            }
            //ˢ��UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.items);
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="itemId">ͼֽid</param>
        /// <returns></returns>
        public bool CheckStock(int itemId)
        {
            BluePrintDetails bluePrintDetails = bluePrintData_SO.GetBluePrintDetails(itemId);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                InventoryItem item = playerBag.GetInventoryItem(resourceItem.itemID);
                if (item.itemAmount > resourceItem.itemAmount)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public GameSaveData GenerateSaveDate()
        {
            GameSaveData gameSaveData = new GameSaveData();
            gameSaveData.playerMoney = this.playerMoney;

            gameSaveData.inventoryDict = new Dictionary<string, List<InventoryItem>>();
            gameSaveData.inventoryDict.Add(playerBag.name, playerBag.items);

            foreach (var item in boxDataDict)
            {
                gameSaveData.inventoryDict.Add(item.Key, item.Value);
            }
            return gameSaveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.playerMoney = saveData.playerMoney;
            playerBag = Instantiate(playerBagTemp);

            playerBag.items = saveData.inventoryDict[playerBag.name];

            foreach (var item in saveData.inventoryDict)
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.items);
        }
    }


}

