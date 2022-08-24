
using MFarm.Transition;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Save { 
    public class DataSlot
    {
        /// <summary>
        /// string GameSaveData的guid
        /// </summary>
        public Dictionary<string, GameSaveData> dataDir = new Dictionary<string, GameSaveData>();


        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.guid;
                if (dataDir.ContainsKey(key))
                {
                    var timeData = dataDir[key];
                    return timeData.timeDict["gameYear"] + "年/" + timeData.timeDict["gameMonth"] + "月/" + timeData.timeDict["gameDay"] + "日";

                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.guid;

                if (dataDir.ContainsKey(key))
                {
                    var transitionData = dataDir[key];
                    return transitionData.dataSceneName;

                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}

