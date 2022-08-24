using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace MFarm.Dialogue
{
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        private NPCMovement npc => GetComponent<NPCMovement>();

        public UnityEvent OnFinishEvent;

        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        private Stack<DialoguePiece> dailogueStack;

        private GameObject uiSign;

        private bool canTalk;
        private bool isTalking;
        private void Awake()
        {
            FillDailoguePiece();
            uiSign = transform.GetChild(1).gameObject;
        }

        private void Update()
        {
            uiSign.SetActive(canTalk);

            if (canTalk & Input.GetKeyDown(KeyCode.Space) && !isTalking)
            {
                //Debug.Log(" Input.GetKeyDown ===========>" + isTalking);
                StartCoroutine(DailogueRoutine());
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = !npc.isMoving && npc.interactable;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = false;
            }
        }

        private void FillDailoguePiece()
        {
            dailogueStack = new Stack<DialoguePiece>();
            for (int i = dialogueList.Count - 1; i > -1; i--)
            {
                dialogueList[i].isDone = false;
                dailogueStack.Push(dialogueList[i]);
            }

        }

        private IEnumerator DailogueRoutine()
        {
            isTalking = true;
            if (dailogueStack.TryPop(out DialoguePiece dialoguePiece))
            {
                EventHandler.CallShowDialogueEvent(dialoguePiece);
                EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                yield return new WaitUntil(()=> dialoguePiece.isDone);
                
                isTalking = false;
            }
            else
            {
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
                EventHandler.CallShowDialogueEvent(null);
                FillDailoguePiece();
                isTalking = false;
                if (OnFinishEvent !=null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;
                }
                
            }
        }
    }
}

