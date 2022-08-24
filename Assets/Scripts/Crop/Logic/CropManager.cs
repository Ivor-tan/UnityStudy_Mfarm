using MFarm.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{
    public class CropManager : Singleton<CropManager>
    {

        public CropDataList_SO cropData;

        private Transform cropParent;
        private Grid currentGrid;

        private Season currentSeason;

        //public Sprite test;


        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }

        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }


        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindGameObjectWithTag("CropParent").transform;
        }

        private void OnGameDayEvent(int gameDay, Season gameSeason)
        {
            currentSeason = gameSeason;
        }

        private void OnPlantSeedEvent(int seedId, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(seedId);
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemId == -1)
            {
                tileDetails.seedItemId = currentCrop.seedItemID;
                tileDetails.growthDays = 0;
                DisplayCropPlant(tileDetails, currentCrop);
                EventHandler.CallDropItemEvent(seedId, new Vector3(), ItemType.Seed);
            }
            else if (tileDetails.seedItemId != -1)//刷新地图
            {
                DisplayCropPlant(tileDetails, currentCrop);
            }

        }

        /// <summary>
        /// 物品id查找物品信息
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int id)
        {
            return cropData.cropDetailsList.Find(details => details.seedItemID == id);
        }

        /// <summary>
        /// 当前季节是否能种
        /// </summary>
        /// <param name="cropDetails"></param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails cropDetails)
        {
            for (int i = 0; i < cropDetails.seasons.Length; i++)
            {
                if (cropDetails.seasons[i] == currentSeason)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 显示作物
        /// </summary>
        /// <param name="tileDetails">地图信息</param>
        /// <param name="cropDetails">作物信息</param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            //倒序计算成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }

            //获取当前阶段prefab
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);
        
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;
            //Debug.Log("cropDetails=========>"+ cropDetails.seedItemID);
        }


    }
}

