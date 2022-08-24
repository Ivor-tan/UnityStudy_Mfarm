
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
         public Image slotHightlight;
        [SerializeField] private Button button;

        [Header("格子类型")]
        public SlotType slotType;

        public bool isSelected;

        public ItemDetails itemDetails;
        public int itemAmount;

        public int slotIndex;

        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        public InventoryLocation Location
        {
            get
            {
                return slotType switch
                {
                    SlotType.Bag => InventoryLocation.Player,
                    SlotType.Box => InventoryLocation.Box,
                    _ => InventoryLocation.Player
                };
            }
        }
        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                UpDateEmptySlot();
            }
        }
        //更新为空
        public void UpDateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateHightlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        //更新格子数据及UI
        public void UpDateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
            
        }

        //点击事件接口
        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null)
            {
                return;
            }
            isSelected = !isSelected;
            inventoryUI.UpdateHightlight(slotIndex);
            //slotHightlight.gameObject.SetActive(isSelected);

            if (slotType == SlotType.Bag)
            {
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            
        }

        //拖拽接口Begin
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemDetails != null)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                //inventoryUI.dragItem.SetNativeSize();
                isSelected = true;
                inventoryUI.UpdateHightlight(slotIndex);
            }
        }
        //拖拽接口OnDrag
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }
        //拖拽接口EndDrag
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;
            GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
            if (gameObject != null)
            {
                if (gameObject.GetComponent<SlotUI>() == null)
                {
                    return;
                }
                else
                {
                    var targetSlot = gameObject.GetComponent<SlotUI>();
                    int targetIndex = targetSlot.slotIndex;


                    if (slotType == SlotType.Bag && targetSlot.slotType==SlotType.Bag)
                    {
                        InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                    }
                    else if (slotType == SlotType.Shop && targetSlot.slotType ==SlotType.Bag)
                    {
                        //买
                        EventHandler.CallShwoTradeUI(itemDetails, false);
                    }
                    else if (slotType == SlotType.Bag && targetSlot.slotType ==SlotType.Shop)
                    {
                        //卖
                        EventHandler.CallShwoTradeUI(itemDetails, true);
                    }
                    else if (slotType != SlotType.Shop && targetSlot.slotType !=SlotType.Shop && slotType !=targetSlot.slotType)
                    {
                        //跨背包交换物品
                        InventoryManager.Instance.SwapItem(Location,slotIndex,targetSlot.Location,targetSlot.slotIndex);
                    }
                    //刷新高亮显示
                    inventoryUI.UpdateHightlight(-1);
                }
            }
            //测试物品扔在世界地图
            //else
            //{
            //    if (itemDetails.canDropped)
            //    {

            //        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            //        EventHandler.CallInstanceItemInScene(itemDetails.itemID, pos);
            //    }

            //}
        }
    }
}