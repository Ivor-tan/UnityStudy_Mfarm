using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteData;

    public List<NPCPosition> NPCPositions;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();


    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }

    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void InitSceneRouteDict()
    {
        if (sceneRouteData.sceneRouteList.Count>0)
        {
            foreach (SceneRoute route in sceneRouteData.sceneRouteList)
            {
                string key = route.fromSceneName + route.goToSceneName;
                if (sceneRouteDict.ContainsKey(key))
                {
                    continue;
                }
                else
                {
                    sceneRouteDict.Add(key, route);
                }
            }
           
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName , string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }

    private void OnStartNewGameEvent(int index)
    {
        foreach (var character in NPCPositions)
        {
            character.npc.position = character.position;
            character.npc.GetComponent<NPCMovement>().StartScene = character.starScene;
        }
    }
}
