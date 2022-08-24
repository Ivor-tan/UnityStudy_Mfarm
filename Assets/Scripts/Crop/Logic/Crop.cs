using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    private int harvestActionCount;

    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;

    private Animator animator;
    private Transform player => FindObjectOfType<Player>().transform;


    public void ProcessToolAction(ItemDetails itemDetails, TileDetails tileDetails)
    {
        this.tileDetails = tileDetails;
        animator = GetComponentInChildren<Animator>();

        int requireActionCount = cropDetails.GetToalRequireCount(itemDetails.itemID);
        if (requireActionCount == -1)
        {
            return;
        }
        //Debug.Log("harvestActionCount============>" + harvestActionCount);
        //计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;
            //收获时效果
            if (animator != null && cropDetails.hasAnimation)
            {
       
                if (player.position.x < transform.position.x)
                {
                    animator.SetTrigger("RotateRight");
                }
                else
                {
                    animator.SetTrigger("RotateLeft");
                }
            }

            //播放粒子
            if (cropDetails.hasParticalEffect)
            {
                EventHandler.CallParticleEffectEvent(cropDetails.effecType, transform.position + cropDetails.effectPos);
            }
            //播放声音
            if (cropDetails.soundName != SoundName.none)
            {
                EventHandler.CallPlaySoundEvent(cropDetails.soundName);
            }

        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)
            {
                //生产作物
                SpawnharvestItem();
            }
            else if (cropDetails.hasAnimation)
            {
                if (player.position.x < transform.position.x)
                {
                    animator.SetTrigger("FallingRight");
                }
                else
                {
                    animator.SetTrigger("FallingLeft");
                }
                EventHandler.CallPlaySoundEvent(SoundName.TreeFalling);
                StartCoroutine(HarvestAfterAnimation());
            }
        }

    }

    private IEnumerator HarvestAfterAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return null;
        }

        SpawnharvestItem();

        if (cropDetails.transferItemID > 0)
        {
            CreateTransferCrop();
        }
    }

    private void CreateTransferCrop()
    {
        tileDetails.seedItemId = cropDetails.transferItemID;
        tileDetails.daysSinceLastHarvest = -1;
        tileDetails.growthDays = 0;

        EventHandler.CallRefreshCurrentMap();

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
                    var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x)*dirX,
                        transform.position.y+ Random.Range(-cropDetails.spawnRadius.y,cropDetails.spawnRadius.y),0);

                    EventHandler.CallInstanceItemInScene(cropDetails.producedItemID[i], spawnPos);

                }
            }

        }

        if (tileDetails != null)
        {
            tileDetails.daysSinceLastHarvest++;
            if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                //刷新种子
                EventHandler.CallRefreshCurrentMap();
            }
            else
            {
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemId = -1;

                //土地是否重置
                //tileDetails.daysSinceDug = -1;
            }

            Destroy(gameObject);
        }
    }
}
