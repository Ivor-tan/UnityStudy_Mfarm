using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;

    public Text dialogueText;
    public Image faceLeft,faceRight;
    public Text nameLeft,nameRight;

    public GameObject continueBox;

    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }

    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void Awake()
    {
        continueBox.SetActive(false);
    }

    private void OnShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        StartCoroutine(ShowDialogue(dialoguePiece));
    }

    private IEnumerator ShowDialogue(DialoguePiece piece)
    {
        if (piece != null)
        {
            piece.isDone = false;
            dialogueBox.SetActive(true);
            continueBox.SetActive(false);
            dialogueText.text = "";
            if (piece.name != string.Empty)
            {
                if (piece.onLeft)
                {
                    faceRight.gameObject.SetActive(false);
                    faceLeft.gameObject.SetActive(true);
                    faceLeft.sprite = piece.faceImage;
                    nameLeft.text = piece.name;
                }
                else
                {
                    faceLeft.gameObject.SetActive(false);
                    faceRight.gameObject.SetActive(true);
                    faceRight.sprite = piece.faceImage;
                    nameRight.text = piece.name;
                }
            }
            else
            {
                faceLeft.gameObject.SetActive(false);
                faceRight.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
            }
            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();
            piece.isDone = true;

            if (piece.hasToPause && piece.isDone)
            {
                continueBox.SetActive(true);
            }
            else
            {
                continueBox.SetActive(false);
            }
        }
        else
        {
            dialogueBox.SetActive(false);
            yield break;
        }
    }
}
