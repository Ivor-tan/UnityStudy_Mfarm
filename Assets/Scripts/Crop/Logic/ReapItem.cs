using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform player => FindObjectOfType<Player>().transform;
        public void InitCropData(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);

        }
        /// <summary>
        /// 生成果实
        /// </summary>
        public void SpawnharvestItem()
        {
            for (int i = 0; i < cropDetails.producedItemID.Length; i++)
            {
                int amountToProduce;
                if (cropDetails.producedMaxAmount[i] == cropDetails.producedMinAmount[i])
                {
                    amountToProduce = cropDetails.producedMaxAmount[i];
                }
                else
                {
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }

                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHavrestAtPlayerPosition(cropDetails.producedItemID[i]);
                    }
                    else
                    {
                        //世界地图生成作物
                        var dirX = transform.position.x > player.position.x ? 1 : -1;
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x) * dirX,
                            transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);

                        EventHandler.CallInstanceItemInScene(cropDetails.producedItemID[i], spawnPos);

                    }
                }

            }
        }
    }
}

