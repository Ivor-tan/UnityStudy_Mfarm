using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;
    public GameObject menuPrefab;

    public Button settingBtn;
    public GameObject pausePanel;
    public Slider volumSlider;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }



    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void Awake()
    {
        settingBtn.onClick.AddListener(TogglePausePanel);
        volumSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }
    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCancas");
        Instantiate(menuPrefab, menuCanvas.transform);
    }


    private void TogglePausePanel()
    {
        
        bool isOpen = pausePanel.activeInHierarchy;

        //Debug.Log("TogglePausePanel====================>"+ isOpen);
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            System.GC.Collect();
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ReturnMenuCanvas()
    {
        Time.timeScale = 1;
        StartCoroutine(BackMenu());
    }

    private IEnumerator BackMenu()
    {
        pausePanel.SetActive(false);
        EventHandler.CallEndGameEvent();
        yield return new WaitForSeconds(1f);
        Instantiate(menuPrefab, menuCanvas.transform);
    }

    private void OnAfterSceneLoadedEvent()
    {
        if (menuCanvas.transform.childCount>0)
        {
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
        }
    }



}
