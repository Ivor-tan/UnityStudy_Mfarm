using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CropDataList_SO", menuName = "Crop/CropDataList")]
public class CropDataList_SO : ScriptableObject
{
    public List<CropDetails> cropDetailsList;
}
