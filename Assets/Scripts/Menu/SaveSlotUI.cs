using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MFarm.Save;

public class SaveSlotUI : MonoBehaviour
{
    public TextMeshProUGUI dataTime, dataScene;

    private Button currentButton;

    private DataSlot currentData;


    private int index => transform.GetSiblingIndex();
    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }

    private void OnEnable()
    {
        SetupSlotUI();
    }

    private void SetupSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[index];
        if (currentData !=null )
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {
            dataTime.text = "没开始世界";
            dataScene.text = "";
        }
    }

    private void LoadGameData()
    {
        //Debug.Log("index==================>"+ index);
        if (currentData !=null)
        {
            SaveLoadManager.Instance.Load(index);
        }
        else
        {
            EventHandler.CallStartNewGameEvent(index);
            Debug.Log("==================>"  +"新游戏");
        }
    }
}
