
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("�����ȡ")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
         public Image slotHightlight;
        [SerializeField] private Button button;

        [Header("��������")]
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
        //����Ϊ��
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

        //���¸������ݼ�UI
        public void UpDateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
            
        }

        //����¼��ӿ�
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

        //��ק�ӿ�Begin
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
        //��ק�ӿ�OnDrag
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }
        //��ק�ӿ�EndDrag
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
                        //��
                        EventHandler.CallShwoTradeUI(itemDetails, false);
                    }
                    else if (slotType == SlotType.Bag && targetSlot.slotType ==SlotType.Shop)
                    {
                        //��
                        EventHandler.CallShwoTradeUI(itemDetails, true);
                    }
                    else if (slotType != SlotType.Shop && targetSlot.slotType !=SlotType.Shop && slotType !=targetSlot.slotType)
                    {
                        //�米��������Ʒ
                        InventoryManager.Instance.SwapItem(Location,slotIndex,targetSlot.Location,targetSlot.slotIndex);
                    }
                    //ˢ�¸�����ʾ
                    inventoryUI.UpdateHightlight(-1);
                }
            }
            //������Ʒ���������ͼ
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