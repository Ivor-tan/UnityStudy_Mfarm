using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using MFarm.Map;
using MFarm.CropPlant;
using MFarm.Inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;

    private Sprite currentSprite;   //存储当前鼠标图片
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private Image buildImage;

    private Camera mainCamera;
    private Grid currentGird;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPostionValid;

    private ItemDetails currentItem;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent; 
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }


    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();

        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);

        currentSprite = normal;
        SetCursorImage(normal);
        mainCamera = Camera.main;

        //Debug.Log("=============>"+ cursorCanvas.ToString());
    }

    private void Update()
    {
        if (cursorCanvas == null) return;

        cursorImage.transform.position = Input.mousePosition;
 
        if (!InteractWithUI() && cursorEnable)
        {
            
            SetCursorImage(currentSprite);
            CheckCursorVaild();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            buildImage.gameObject.SetActive(false);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        currentGird = FindObjectOfType<Grid>();
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
       
        if (!isSelected)
        {
            cursorEnable = false;
            currentItem = null;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else    //物品被选中才切换图片
        {
            currentItem = itemDetails;
            //WORKFLOW:添加所有类型对应图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                _ => normal,
            };
            cursorEnable = true;

            if (itemDetails.itemType == ItemType.Furniture)
            {
                //Debug.Log("OnItemSelectedEvent  =====================>");
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
                //原图太大
                //buildImage.transform.localScale = buildImage.transform.localScale * 0.5f;
            }
        }
    }


    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPostionValid)
        {
            //Debug.Log("GetMouseButtonDown  ===============>");
            //鼠标点击方法
            EventHandler.CallMouseClickEvent(mouseWorldPos, currentItem);
        }
    }

    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void setCursorValid()
    {
        cursorPostionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
    }

    private void setCursorInValid()
    {
        cursorPostionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.45f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
    }

    #endregion

    /// <summary>
    /// 物品选择事件函数
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void CheckCursorVaild()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGridPos = currentGird.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGird.WorldToCell(PlayerTransform.position);

        //建造图片跟随
        buildImage.transform.position = Input.mousePosition;

        //判断使用范围
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            setCursorInValid();
            return;
        }

        TileDetails currcentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currcentTile !=null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currcentTile.seedItemId);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currcentTile.daysSinceDug > -1 && currcentTile.seedItemId == -1)
                    {
                        setCursorValid();
                    }
                    else
                    {
                        setCursorInValid();
                    }
                    break;
                case ItemType.Commodity:
                    if (currcentTile.canDropItem && currentItem.canDropped)
                    {
                        setCursorValid();
                    }
                    else
                    {
                        setCursorInValid();
                    }
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    var bluePrintDetails = InventoryManager.Instance.bluePrintData_SO.GetBluePrintDetails(currentItem.itemID);
                    if (currcentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID) && !HaveFurnitureInRadius(bluePrintDetails))
                    {
                        setCursorValid();
                    }
                    else
                    {
                        setCursorInValid();
                    }

                    break;
                case ItemType.HoeTool:
                    if (currcentTile.canDig)
                    {
                        setCursorValid();
                    }
                    else
                    {
                        setCursorInValid();
                    }

                    break;
            
                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemInRadius(mouseWorldPos,currentItem))
                    {
                        setCursorValid();
                    }
                    else
                    {
                        setCursorInValid();
                    }
                    break;
                case ItemType.WaterTool:
                    if (currcentTile.daysSinceDug > -1 && currcentTile.daysSinceWatered == -1)
                    {
                        setCursorValid();
                    }
                    else
                    {
                        setCursorInValid();
                    }
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    if (crop != null )
                    {
                        if (crop.CanHarvest && crop.cropDetails.CheckoutToolAvailable(currentItem.itemID) )
                        {
                            setCursorValid();
                        }
                    }
                    else
                    {
                        setCursorInValid();
                    }

                    break;
                case ItemType.CollectTool:
                    if (currentCrop !=null)
                    {
                        if (currentCrop.CheckoutToolAvailable(currentItem.itemID))
                        {
                            if (currcentTile.growthDays >= currentCrop.TotalGrowthDays)
                            {
                                setCursorValid();
                            }
                        }
                    
                    }
                    else
                    {
                        setCursorInValid();
                    }

                    break;
                case ItemType.ReapableScenery:
                    break;
                default:
                    break;
            }
        }
        else
        {
            setCursorInValid();
        }
    }

    private bool HaveFurnitureInRadius(BluePrintDetails bluePrintDetails)
    {
        var buildItem = bluePrintDetails.buildPrefab;
        Vector2 point = mouseWorldPos;
        var size = buildItem.GetComponent<BoxCollider2D>().size;

        var otherColl = Physics2D.OverlapBox(point, size, 0);
        if (otherColl != null)
            return otherColl.GetComponent<Furniture>();
        return false;
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
