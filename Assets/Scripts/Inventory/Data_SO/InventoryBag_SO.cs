using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InventoryBag_SO",menuName = "Inventory/InventoryBag_SO")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> items;

    public InventoryItem GetInventoryItem(int id)
    {
       return items.Find(item => item.itemID == id);
    }
}
