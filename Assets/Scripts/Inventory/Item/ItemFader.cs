using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //�ָ���ɫ
    public void FaderIn()
    {
        Color targetColor = new Color(1,1,1,1);
        spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }

    //��͸��
    public void FaderOut()
    {
        Color targetColor = new Color(1, 1, 1, Settings.targetAlpha);
        spriteRenderer.DOColor(targetColor, Settings.itemFadeDuration);
    }
}

