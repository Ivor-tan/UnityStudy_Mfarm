using UnityEngine;
[System.Serializable]
public class CropDetails
{
    [Header("������Ϣ")]
    public int seedItemID;
    public string seedName;
    [Header("��ͬ�׶���Ҫ������")]
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

    [Header("��ͬ�����׶���ƷPrefab")]
    public GameObject[] growthPrefabs;
    [Header("��ͬ�׶ε�ͼƬ")]
    public Sprite[] growthSprites;
    [Header("����ֲ�ļ���")]
    public Season[] seasons;

    [Space]
    [Header("�ո��")]
    public int[] harvestToolItemID;
    [Header("ÿ�ֹ���ʹ�ô���")]
    public int[] requireActionCount;
    [Header("ת������ƷID")]
    public int transferItemID;

    [Space]
    [Header("�ո��ʵ��Ϣ")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("�ٴ�����ʱ��")]
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
    /// ��鹤���Ƿ����
    /// </summary>
    /// <param name="toolId">����ID</param>
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
    /// ��ȡ����ʹ�ô���
    /// </summary>
    /// <param name="toolId">����id</param>
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
