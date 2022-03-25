using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class StarManager : Singleton<StarManager>
{
    [SerializeField] GameObject starPrefab;
    [SerializeField] Transform canvasTransform,starIndicatorTransform;
    [SerializeField] TextMeshProUGUI starAmountText;

    private int totalEarnedStars;


    private void Start()
    {
        totalEarnedStars = 0;
        UpdateEarnedStarAmount();
    }


    public void MoveStarToStarIndicator(Vector3 initalPos)
    {
        int currentCombo = ComboManager.Instance.GetCurrentCombo();

        GameObject[] stars = new GameObject[currentCombo];
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < currentCombo; i++)
        {
          
            GameObject star = Instantiate(starPrefab, canvasTransform, false);
            //GameObject star = Instantiate(starPrefab, initalPos, Quaternion.identity);
            star.SetActive(false);
            star.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.02f);
            star.layer = LayerMask.NameToLayer("UI");
            star.transform.SetParent(canvasTransform);
            star.SetActive(true);
            star.transform.position = initalPos + new Vector3(0, 0.7f, 0);
            seq.Join(star.transform.DOMove(starIndicatorTransform.position, 0.7f+(i*0.1f)).OnComplete(()=>Destroy(star)));
            totalEarnedStars += 1;
            UpdateEarnedStarAmount();
        }

        seq.Play();


        ComboManager.Instance.IncreaseCombo();
    }

    private void UpdateEarnedStarAmount()
    {

        starAmountText.text = totalEarnedStars.ToString();
    }
}
