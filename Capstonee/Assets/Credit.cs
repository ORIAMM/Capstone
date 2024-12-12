using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Credit : MonoBehaviour
{
    [SerializeField] private RectTransform InputMenu;
    [SerializeField] private RectTransform credit;


    public void OnCredit()
    {
        InputMenu.DOAnchorPosY(820, 1.5f).SetEase(Ease.InOutSine);
        credit.DOAnchorPosY(0, 1.5f).SetEase(Ease.InOutSine);
        
    }

    public void OffCredit()
    {
        InputMenu.DOAnchorPosY(-88, 1.5f).SetEase(Ease.InOutSine);
        credit.DOAnchorPosY(-900, 1.5f).SetEase(Ease.InOutSine);
    }
}
