using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;
public class Player : MonoBehaviour,ISaveable

{

    private Rigidbody2D rb;

    public float speed;
    private float inputX;
    private float inputY;

    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;
    private bool inputDisable;

    //使用工具
    private float mouseX;
    private float mouseY;
    private bool useTool;

    public string guid => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.MouseClickEvent += OnMouseClickEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }



    private void OnDisable()
    {
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.MouseClickEvent -= OnMouseClickEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        inputDisable = true;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void Update()
    {
        if (!inputDisable)
        {
            PlayerInput();
        }
        else
        {
            isMoving = false;
        }
       
        SwitchAnimation();
    }


    private void FixedUpdate()
    {
        if (!inputDisable)
        {
            Movement();
        }
 
    }

    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        //走路状态速度
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput = new Vector2(inputX,inputY);
        isMoving = (movementInput != Vector2.zero);
    }


    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    private void SwitchAnimation()
    {
        foreach (var animator in animators)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetFloat("mouseX", mouseX);
            animator.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                animator.SetFloat("InputX", inputX);
                animator.SetFloat("InputY", inputY);
            }
        }
    }

    private void OnEndGameEvent()
    {
        inputDisable = true;
    }

    private void OnStartNewGameEvent(int index)
    {
        inputDisable = false;
        transform.position = Settings.playerStartPos;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }

    private void OnAfterSceneLoadEvent()
    {
        inputDisable = false;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnMouseClickEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //Debug.Log(" OnMouseClickEvent =====================>" + itemDetails.itemID);
        //鼠标点击之后需要运行的方法
        if (itemDetails.itemType != ItemType.Seed 
            && itemDetails.itemType != ItemType.Commodity 
            && itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);

            if (Mathf.Abs(mouseX)> Mathf.Abs(mouseY))
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }

    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GamePlay:
                inputDisable = false;
                break;
            case GameState.Pause:
                inputDisable = true;
                break;
            default:
                break;
        }
    }
    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetTrigger("useTool");
            //改变任务方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);

        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);

        yield return new WaitForSeconds(0.25f);

        useTool = false;
        inputDisable = false;

    }

    public GameSaveData GenerateSaveDate()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        gameSaveData.characterPosDict.Add(this.name,new SerializableVector3(transform.position));
        return gameSaveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        var targetPosition = saveData.characterPosDict[this.name].ToVector();
        transform.position = targetPosition;
    }


}
