using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;

namespace MFarm.Transition
{

    public class TransitionManager : Singleton<TransitionManager>,ISaveable
    {
        [SceneName]
        public string startSceneName = string.Empty;
        private bool isFade;

        private CanvasGroup fadeCanvasGroup;

        public string guid => GetComponent<DataGUID>().guid;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private void Start()
        {
            ((ISaveable)this).RegisterSaveable();
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
            //yield return StartCoroutine(LoadSceneSetActive(startSceneName));
            //Debug.Log("Start  ==================>");
            //EventHandler.CallAfterSceneLoadEvent();
        }

        private void OnEnable()
        {
            EventHandler.TranitionEvent += OnTransitionEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.TranitionEvent -= OnTransitionEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;
        }



        private void OnStartNewGameEvent(int index)
        {
            StartCoroutine(LoadSaveDataScene(startSceneName));
        }

        private void OnTransitionEvent(string sceneName, Vector3 pos)
        {
            if (!isFade)
            {
                StartCoroutine(Transition(sceneName, pos));
            }
        }



        //加载设置激活
        private IEnumerator LoadSceneSetActive(string sceneName)
        {

            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
            //Debug.Log("LoadSceneSetActive  ==================>");
            //EventHandler.CallAfterSceneLoadEvent();
        }

        private IEnumerator Transition(string sceneName,Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);

            EventHandler.CallMoveToPosition(targetPosition);

            //Debug.Log("Transition  ==================>");
            EventHandler.CallAfterSceneLoadEvent();

            yield return Fade(0);

        }

        //场景淡入淡出
        private IEnumerator Fade(float targetAlpha)
        {

            isFade = true;

            fadeCanvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha)/Settings.fadeDuration;

            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }

            fadeCanvasGroup.blocksRaycasts = false;

            isFade = false;
        }


        private IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return Fade(1f);

            if (SceneManager.GetActiveScene().name != "PersistentScene")
            {
                EventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }

            yield return LoadSceneSetActive(sceneName);

            EventHandler.CallAfterSceneLoadEvent();

            yield return Fade(0);

        }
        public GameSaveData GenerateSaveDate()
        {
            GameSaveData gameSaveData = new GameSaveData();
            gameSaveData.dataSceneName = SceneManager.GetActiveScene().name;
            return gameSaveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            //加载场景 

            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }

        private IEnumerator UnlaodScene()
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return Fade(0);
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnlaodScene());
        }
    }
}