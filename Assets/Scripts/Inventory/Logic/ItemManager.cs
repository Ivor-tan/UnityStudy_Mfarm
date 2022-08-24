using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour,ISaveable
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;

        private Transform itemParent;

        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();

        private Transform plyerTrans => FindObjectOfType<Player>().transform;

        public string guid => GetComponent<DataGUID>().guid;

     
        private void OnEnable()
        {
            EventHandler.InstanceItemInScene += OnInstanceItemInScene;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
            EventHandler.BuildFurntureEvent += OnBuildFurntureEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }
        private void Start()
        {
            ((ISaveable)this).RegisterSaveable();
        }

        private void OnDisable()
        {
            EventHandler.InstanceItemInScene -= OnInstanceItemInScene;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
            EventHandler.BuildFurntureEvent -= OnBuildFurntureEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }

        private void OnStartNewGameEvent(int index)
        {
            sceneItemDict.Clear();
            sceneFurnitureDict.Clear();
        }

        private void OnBeforeSceneUnloadEvent()
        {
            //Debug.Log("OnBeforeSceneUnloadEvent =====================>");
            GetAllSceneItem();
            GetAllSceneFurniture();
        }

        private void OnAfterSceneLoadEvent()
        {
            //Debug.Log("OnAfterSceneLoadEvent =====================>");
            itemParent = GameObject.FindGameObjectWithTag("ItemParent").transform;
            RecreateAllItem();
            RecreateAllFurniture();
        }

        private void OnInstanceItemInScene(int id, Vector3 pos)
        {
            //
            var item = Instantiate(bounceItemPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, itemParent);
            item.itemID = id;

            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
        }

        private void OnDropItemEvent(int itemId, Vector3 mousePos,ItemType itemType)
        {
            //Debug.Log("=============>"+ itemId);
            if (itemType == ItemType.Seed)
            {
                return;
            }

            var item = Instantiate(bounceItemPrefab, plyerTrans.position, Quaternion.identity, itemParent);
            item.itemID = itemId;

            var dir = (mousePos - plyerTrans.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);

        }

        private void OnBuildFurntureEvent(int id ,Vector3 mousePos)
        {
            BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData_SO.GetBluePrintDetails(id);
            var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);
            if (buildItem.GetComponent<Box>())
            {
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                buildItem.GetComponent<Box>().initBox(buildItem.GetComponent<Box>().index);
            }
  
        }

        //获取场景中所有物品
        private void GetAllSceneItem()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();

            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemId = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneItem.Add(sceneItem);
            }

            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItem;
            }
            else
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItem);
            }
        }

        //重新创建场景中所有物品
        private void RecreateAllItem()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name,out currentSceneItem))
            {
                if (currentSceneItem !=null)
                {
                    //删掉原有的物品
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    foreach (var item in currentSceneItem)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemId);
                    }
                    
                }
            }
        }


        //获取场景中所有建筑
        private void GetAllSceneFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();
            
            foreach (Furniture item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture
                {
                    itemId = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                if (item.GetComponent<Box>())
                {
                    sceneFurniture.boxIndex = item.GetComponent<Box>().index;
                }
                currentSceneFurniture.Add(sceneFurniture);
            }
            //Debug.Log("GetAllSceneFurniture =====================>" + currentSceneFurniture.Count);

            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
            }
            else
            {
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }

        private void RecreateAllFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();
            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {

                if (currentSceneFurniture != null)
                {
                    //删掉原有的物品
                    foreach (var item in FindObjectsOfType<Furniture>())
                    {
                        Destroy(item.gameObject);
                    }

                    foreach (SceneFurniture furniture in currentSceneFurniture)
                    {
                        //Debug.Log("OnBuildFurntureEvent =====================>" + currentSceneFurniture.Count);
                        //OnBuildFurntureEvent(furniture.itemId, furniture.position.ToVector());
                        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData_SO.GetBluePrintDetails(furniture.itemId);
                        var buildItem = Instantiate(bluePrint.buildPrefab, furniture.position.ToVector(), Quaternion.identity, itemParent);
                        if (buildItem.GetComponent<Box>())
                        {
                            buildItem.GetComponent<Box>().initBox(furniture.boxIndex);
                        }
                    }

                }
            }
        }

        public GameSaveData GenerateSaveDate()
        {
            GetAllSceneFurniture();
            GetAllSceneItem();
            GameSaveData gameSaveData = new GameSaveData();
            gameSaveData.sceneItemDict = this.sceneItemDict;
            gameSaveData.sceneFurnitureDict = this.sceneFurnitureDict;

            return gameSaveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.sceneItemDict = saveData.sceneItemDict;
            this.sceneFurnitureDict = saveData.sceneFurnitureDict;
            RecreateAllItem();
            RecreateAllFurniture();
        }
    }

}
