using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singleton<TimelineManager>
{
    public PlayableDirector startDirector;
    public PlayableDirector currentDirector;

    private bool isPause;
    public bool isDone;

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    private void OnEnable()
    {
        //回调执行不到
        //currentDirector.played += TimelinePlayed;
        //currentDirector.stopped += TimelineStopped;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    
    }



    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }

    //private void TimelinePlayed(PlayableDirector director)
    //{
    //    if (director != null)
    //    {
    //        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    //    }
    //}

    //private void TimelineStopped(PlayableDirector director)
    //{
    //    if (director != null)
    //    {
    //        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    //        director.gameObject.SetActive(false);
    //    }
    //}

    private void OnAfterSceneLoadedEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if (currentDirector!=null)
        {
            currentDirector.Play();
        }
    }
}
