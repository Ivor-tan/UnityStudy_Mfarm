using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>,ISaveable
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;

        [Header("蓝图数据")]
        public BluePrintData_SO bluePrintData_SO;

        [Header("背包数据")]
        public InventoryBag_SO playerBag;

        public InventoryBag_SO playerBagTemp;

        private InventoryBag_SO currentBoxBag;

        [Header("交易相关")]
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

        //获取物品信息
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        //添加物品，销毁
        public void AddItem(Item item ,bool toDestory)
        {
            //背包空位，是否有该物品
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            if (toDestory)
            {
                Destroy(item.gameObject);
            }
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.items);
        }

        //背包空位
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

        //是否有该物品
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

        //指定位置添加物品
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

        //交换物品
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

                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)  //有不相同的两个物品
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //相同的两个物品
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else    //目标空格子
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
        /// 查找箱子数据
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
        /// 加入箱子数据字典
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
        /// 在  玩家背包  中移除指定数量物品
        /// </summary>
        /// <param name="ID">物品id</param>
        /// <param name="removeAmount">数量</param>
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
            //移除背包物品
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
        /// 打开背包时获取box数据
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bag_SO"></param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag = bag_SO;
        }

        /// <summary>
        /// 交易物品
        /// </summary>
        /// <param name="itemDetails">物品信息</param>
        /// <param name="amount">交易数量</param>
        /// <param name="isSellTrade">是否卖东西</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //获得物品背包位置
            int index = GetItemIndexInBag(itemDetails.itemID);

            if (isSellTrade)    //卖
            {
                if (playerBag.items[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    //卖出总价
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney - cost >= 0)   //买
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;
            }
            //刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.items);
        }

        /// <summary>
        /// 检查库存
        /// </summary>
        /// <param name="itemId">图纸id</param>
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

