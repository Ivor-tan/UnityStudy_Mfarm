using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save
{
    [System.Serializable]
    public class GameSaveData
    {
        public string dataSceneName;
        /// <summary>
        /// �洢�������꣬string��������
        /// </summary>
        public Dictionary<string, SerializableVector3> characterPosDict;
        public Dictionary<string, List<SceneItem>> sceneItemDict;
        public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;
        public Dictionary<string, TileDetails> tileDetailsDict;
        public Dictionary<string, bool> firstLoadDict;
        public Dictionary<string, List<InventoryItem>> inventoryDict;

        public Dictionary<string, int> timeDict;

        public int playerMoney;

        //NPC
        public string targetScene;
        public bool interactable;
        public int animationInstanceID;
    }
}
