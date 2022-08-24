using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Transition
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneName;

        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                EventHandler.CallTranitionEvent(sceneName, positionToGo);
            }
        }
    }
}
