using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;

    public SpriteRenderer holdItem;


    [Header("各个部分动画列表")]
    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (var animator in animators)
        {
            animatorNameDict.Add(animator.name, animator);
        }

    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.HavrestAtPlayerPosition += OnHavrestAtPlayerPosition;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.HavrestAtPlayerPosition -= OnHavrestAtPlayerPosition;
    }

    private void OnHavrestAtPlayerPosition(int id)
    {
        Sprite sprite = InventoryManager.Instance.GetItemDetails(id).itemOnWorldSprite;
        holdItem.sprite = sprite;
        if (holdItem.enabled == false)
        {
            StartCoroutine(ShowItem());
        }
    }

    private IEnumerator ShowItem()
    {
        holdItem.enabled = true;
        yield return new WaitForSeconds(1.0f);
        holdItem.enabled = false;

    }
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        
        SwitchAnimator(PartType.None);//解决切换物品动画没有还原问题
        //不同物品播放的动画
        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            ItemType.HoeTool => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            ItemType.ChopTool => PartType.Chop,
            ItemType.BreakTool => PartType.Break,
            ItemType.ReapTool => PartType.Reap,
            ItemType.Furniture => PartType.Carry,
            _ => PartType.None,
        };
       
        if (!isSelected)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if (currentType == PartType.Carry)
            {
                holdItem.enabled = true;
                holdItem.sprite = itemDetails.itemOnWorldSprite;
            }
            else
            {
                holdItem.enabled = false;
            }
        }
        SwitchAnimator(currentType);
    }

    private void SwitchAnimator(PartType partType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

}
