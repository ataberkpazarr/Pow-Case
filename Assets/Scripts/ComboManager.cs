using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ComboManager : Singleton<ComboManager>
{

    [SerializeField] private Image comboFillBar;
    [SerializeField] private float maxTime = 5f;
    [SerializeField] private TextMeshProUGUI comboText;

    private bool comboActive = false;

    private float timeLeft;
    private int currentCombo =1;

    private void Start()
    {
        timeLeft = maxTime;
    }
    void Update()
    {
        if (comboActive)
        {


            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                comboFillBar.fillAmount = timeLeft / maxTime;
            }
            else if (timeLeft <= 0)
            {
               
                comboActive = false;

            }
        }
        else
        {
            
            comboFillBar.fillAmount = 0;
            currentCombo = 1;
            UpdateComboText();
        }

        
    }

    private void StartComboBar()
    {
        timeLeft = maxTime;
        //comboFillBar.fillAmount = 1;
    }

    public void IncreaseCombo()
    {
        if (comboActive)
        {
            

        }
        else
        {
            comboActive = true; 
        }
        StartComboBar();
        currentCombo += 1;
        UpdateComboText();

    }

    private void UpdateComboText()
    {
        comboText.text = "x" + currentCombo.ToString();

    }

    public int GetCurrentCombo()
    {
        return currentCombo;
    }
    public void StopCombo()
    {
        comboActive = false;
    }
}
