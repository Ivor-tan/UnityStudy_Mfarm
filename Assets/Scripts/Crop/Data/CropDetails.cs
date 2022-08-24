using UnityEngine;
[System.Serializable]
public class CropDetails
{
    [Header("种子信息")]
    public int seedItemID;
    public string seedName;
    [Header("不同阶段需要的天数")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("不同生长阶段物品Prefab")]
    public GameObject[] growthPrefabs;
    [Header("不同阶段的图片")]
    public Sprite[] growthSprites;
    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemID;
    [Header("每种工具使用次数")]
    public int[] requireActionCount;
    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("再次生长时间")]
    public int daysToRegrow;
    public int regrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;

    public ParticleEffecType effecType;
    public Vector3 effectPos;
    public SoundName soundName;
    /// <summary>
    /// 检查工具是否可用
    /// </summary>
    /// <param name="toolId">工具ID</param>
    /// <returns></returns>
    public bool CheckoutToolAvailable(int toolId)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (toolId ==tool)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取工具使用次数
    /// </summary>
    /// <param name="toolId">工具id</param>
    /// <returns></returns>
    public int GetToalRequireCount(int toolId)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (toolId == harvestToolItemID[i])
            {
                return requireActionCount[i];
            }
        }

        return -1;
    }

}
