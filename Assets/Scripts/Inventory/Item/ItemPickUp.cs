using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("OnTriggerEnter2D=========>  " + collision.gameObject.name);
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                if (item.itemDetails.canPickedup)
                {
                    InventoryManager.Instance.AddItem(item, true);

                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }

    }
}

