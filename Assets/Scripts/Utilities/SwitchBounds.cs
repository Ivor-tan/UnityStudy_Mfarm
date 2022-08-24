using System;
using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfinerShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }

    //切换场景后调用
    //void Start()
    //{
    //    //切换场景后调用
    //    SwitchConfinerShape();
    //}



    //移除 CM vcam1下面的camer组件
    private void SwitchConfinerShape()
    {
        if (GameObject.FindGameObjectWithTag("BoundsConfiner") != null)
        {
            PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

            CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

            confiner.m_BoundingShape2D = confinerShape;

            confiner.InvalidatePathCache();
        }
    }
}
