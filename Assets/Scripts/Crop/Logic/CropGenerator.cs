using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;
using System;

namespace MFarm.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;
        public int seedId;
        public int growthDay;

        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += OnGenerateCropEvent;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= OnGenerateCropEvent;
        }

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);
            if (seedId!=0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);
                if (tile == null)
                {
                    tile = new TileDetails();
                    tile.gridX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }
                tile.daysSinceWatered = -1;
                tile.seedItemId = seedId;
                tile.growthDays = this.growthDay;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }

        private void OnGenerateCropEvent()
        {
            GenerateCrop();
        }

    }
}

