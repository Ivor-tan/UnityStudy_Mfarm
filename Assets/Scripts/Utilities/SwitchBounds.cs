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

    //�л����������
    //void Start()
    //{
    //    //�л����������
    //    SwitchConfinerShape();
    //}



    //�Ƴ� CM vcam1�����camer���
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
