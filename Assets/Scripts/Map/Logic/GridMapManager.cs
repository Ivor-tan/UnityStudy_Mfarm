using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MFarm.CropPlant;
using MFarm.Save;
namespace MFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>,ISaveable
    {

        [Header("�ֵ���Ƭ��ͼ��Ϣ")]
        public RuleTile digTile;
        public RuleTile waterTile;

        private Tilemap digTilemap;
        private Tilemap waterTileMap;

        [Header("��ͼ��Ϣ")]
        public List<MapData_SO> mapDataList;

        private Season currentSeason;


        //������+����  ��Ӧ��Ƭ��Ϣ
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();

        private Grid currentGrid;

        private List<ReapItem> reapItems;

        public string guid => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += OnRefreshCurrentMap;
        }

        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= OnRefreshCurrentMap;
        }

 
        private void Start()
        {

            ((ISaveable)this).RegisterSaveable();

            foreach (var item in mapDataList)
            {
                //Debug.Log("Start ===========>" + item.sceneName);
                firstLoadDict.Add(item.sceneName, true);
                InitTileDetailsDict(item);
            }
        }



        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty item in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX =item.tileCoordinate.x,
                    gridY =item.tileCoordinate.y
                };

                string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;
                if (GetTileDetails(key)!=null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (item.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = item.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = item.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = item.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = item.boolTypeValue;
                        break;
                    default:
                        break;
                }
                if (GetTileDetails(key) != null)
                {
                    tileDetailsDict[key] = tileDetails;
                }
                else
                {
                    tileDetailsDict.Add(key, tileDetails);
                }

            }
        }


        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }

        //�������귵�ص�ͼ��Ϣ
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            
            return GetTileDetails(key);
        }

        /// <summary>
        /// ��ʾ�ڵ� ��ͼ
        /// </summary>
        /// <param name="tileDetails"></param>
        private void setDigGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridX, tileDetails.gridY, 0);
            if (digTilemap != null)
            {
                digTilemap.SetTile(pos, digTile);
            }

        }

        /// <summary>
        /// ��ʾ��ˮ��ͼ
        /// </summary>
        /// <param name="tileDetails"></param>
        private void setWaterGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridX, tileDetails.gridY, 0);
            if (waterTileMap != null)
            {
                waterTileMap.SetTile(pos, waterTile);
            }
        }

        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }


                //�����ڿ�����
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemId == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }

                if (tile.Value.seedItemId > -1)
                {
                    tile.Value.growthDays++;
                }
            }

            RefreshMap();
            //DisplayMap();
        }
        private void OnExecuteActionAfterAnimation(Vector3 mousePos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mousePos);
            TileDetails currentTitle = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTitle !=null)
            {
                Crop currentCrop = GetCropObject(mousePos);
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTitle);
                        //���Ӳ�����ʱ���ֱ������٣�����û������
                        //EventHandler.CallDropItemEvent(itemDetails.itemID, mousePos, itemDetails.itemType);
                        EventHandler.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mousePos, itemDetails.itemType);

                        break;
                    case ItemType.Furniture:
                        //�ڵ�ͼ������Ʒ����
                        //�Ƴ������Ʒ
                        EventHandler.CallBuildFurntureEvent(itemDetails.itemID, mousePos);

                        break;
                    case ItemType.HoeTool:
                        setDigGround(currentTitle);
                        currentTitle.daysSinceDug = 0;
                        currentTitle.canDig = false;
                        currentTitle.canDropItem = false;
                        EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.ReapTool:
                        //Debug.Log("==============>" + reapItems.Count);
                        for (int i = 0; i < reapItems.Count; i++)
                        {
                            //Debug.Log("==============>" + reapItems[i].name);
                            EventHandler.CallParticleEffectEvent(ParticleEffecType.ReapableSecnery, reapItems[i].transform.position + Vector3.up);
                            reapItems[i].SpawnharvestItem();
                            Destroy(reapItems[i].gameObject);
                            if (i > Settings.raepAmount)
                            {
                                break;
                            }
                        }
                        EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        break;
                    case ItemType.WaterTool:
                        setWaterGround(currentTitle);
                        currentTitle.daysSinceWatered = 0;
                        currentTitle.canDig = false;
                        currentTitle.canDropItem = false;
                        EventHandler.CallPlaySoundEvent(SoundName.Water);
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        //�ո��
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                  
                        break;
                    case ItemType.CollectTool:
                        //Debug.Log("=========>" + currentCrop.cropDetails.seedItemID);
                        //�ո��
                        currentCrop.ProcessToolAction(itemDetails, currentTitle);

                        break;
                    case ItemType.ReapableScenery:
                        break;
                    default:
                        break;
                }

                UpdateTileDetails(currentTitle);
            }
        }

        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTilemap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            //Debug.Log("OnAfterSceneLoadEvent ===========>"+ SceneManager.GetActiveScene().name);
            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                EventHandler.CallGenerateCropEvent();
            }

            RefreshMap();
            firstLoadDict[SceneManager.GetActiveScene().name] = false;
            //DisplayMap(SceneManager.GetActiveScene().name);
        }

        private void OnRefreshCurrentMap()
        {
            RefreshMap();
        }

        /// <summary>
        /// ������Ƭ��ͼ��Ϣ
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }


        private void RefreshMap()
        {
            if (digTilemap != null)
            {
                digTilemap.ClearAllTiles();
            }
            if (waterTileMap != null)
            {
                waterTileMap.ClearAllTiles();
            }

            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }


        /// <summary>
        /// ��ʾ��ͼ��Ϣ
        /// </summary>
        /// <param name="sceneName"></param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                string key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                    {
                        setDigGround(tileDetails);
                    }
                    if (tileDetails.daysSinceWatered > -1)
                    {
                        setWaterGround(tileDetails);
                    }

                    if (tileDetails.seedItemId >-1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemId, tileDetails);
                    }
                    //������Ϣ

                }
            }
        }

        /// <summary>
        /// ����ж��Ƿ��������
        /// </summary>
        /// <param name="mouseWorldPos">��������</param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                {
                    currentCrop = colliders[i].GetComponent<Crop>();
                }
            }

            return currentCrop;
        }

        public bool HaveReapableItemInRadius(Vector3 mouseWorldPos, ItemDetails tool)
        {
            reapItems = new List<ReapItem>();
            Collider2D[] colliders = new Collider2D[20];

            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        var item = colliders[i].GetComponent<ReapItem>();
                        if (item !=null)
                        {
                            reapItems.Add(item);
                        }
                        //Debug.Log("HaveReapableItemInRadius=========>" + item);
                   
                    }
                }
            }

            return reapItems.Count > 0;
        }

        /// <summary>
        /// ���ݳ������ֹ�������Χ�������Χ��ԭ��
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="gridDimensions">����Χ</param>
        /// <param name="gridOrigin">����ԭ��</param>
        /// <returns>�Ƿ��е�ǰ��������Ϣ</returns>
        public bool GetGridDimensions(string sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapData in mapDataList)
            {
                if (mapData.sceneName == sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;

                    gridOrigin.x = mapData.originX;
                    gridOrigin.y = mapData.originY;

                    return true;
                }
            }
            return false;
        }

        public GameSaveData GenerateSaveDate()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.tileDetailsDict = this.tileDetailsDict;
            saveData.firstLoadDict = this.firstLoadDict;
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.tileDetailsDict = saveData.tileDetailsDict;
            this.firstLoadDict = saveData.firstLoadDict;
        }
    }
}
