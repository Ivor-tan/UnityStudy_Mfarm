using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MFarm.Inventory;
using MFarm.Dialogue;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation inventoryLocation, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(inventoryLocation, list);
    }

    public static event Action<int, Vector3> InstanceItemInScene;
    public static void CallInstanceItemInScene(int ID, Vector3 pos)
    {
        InstanceItemInScene?.Invoke(ID, pos);
    }

    public static event Action<int, Vector3, ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos,ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails item, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(item, isSelected);
    }

    public static event Action<int, int, int ,Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour,int day,Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour, day, season);
    }

    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }

    public static event Action<int, int,int,int,Season> GameDateEvent;
    public static void CallGameDateEvent(int hour, int day,int month,int year,Season season)
    {
        GameDateEvent?.Invoke(hour, day,month,year,season);
    }

    public static event Action<string,Vector3> TranitionEvent;
    public static void CallTranitionEvent(string  sceneName, Vector3 pos)
    {
        TranitionEvent?.Invoke(sceneName, pos);
    }


    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }


    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }


    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 pos)
    {
        MoveToPosition?.Invoke(pos);
    }


    public static event Action<Vector3, ItemDetails> MouseClickEvent;

    public static void CallMouseClickEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickEvent?.Invoke(pos,itemDetails);
    }

    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;

    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    public static event Action<int, TileDetails> PlantSeedEvent;

    public static void CallPlantSeedEvent(int seedId,TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(seedId, tileDetails);
    }


    public static event Action<int> HavrestAtPlayerPosition;
    public static void CallHavrestAtPlayerPosition(int itemId)
    {
        HavrestAtPlayerPosition?.Invoke(itemId);
    }

    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static event Action<ParticleEffecType,Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticleEffecType particleEffecType ,Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(particleEffecType, pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);
    }

    public static event Action<SlotType,InventoryBag_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType, InventoryBag_SO inventoryData)
    {
        BaseBagOpenEvent?.Invoke(slotType, inventoryData);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO inventoryData)
    {
        BaseBagCloseEvent?.Invoke(slotType, inventoryData);
    }


    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }

    public static event Action<ItemDetails ,bool> ShwoTradeUI;
    public static void CallShwoTradeUI(ItemDetails item ,bool isSell)
    {
        ShwoTradeUI?.Invoke(item, isSell);
    }

    //建造
    public static event Action<int, Vector3 > BuildFurntureEvent;
    public static void CallBuildFurntureEvent(int id, Vector3 mousePos)
    {
        BuildFurntureEvent?.Invoke(id, mousePos);
    }

    //灯光
    public static event Action<Season ,LightShift ,float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(Season season, LightShift lightShift ,float timeDifference )
    {
        LightShiftChangeEvent?.Invoke(season, lightShift, timeDifference);
    }

    //音效
    public static event Action<SoundDetails> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetails soundDetails)
    {
        InitSoundEffect?.Invoke(soundDetails);
    }

    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName)
    {
        PlaySoundEvent?.Invoke(soundName);
    }

    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }

    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }
}
