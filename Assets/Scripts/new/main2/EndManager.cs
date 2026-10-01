using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndManager : MonoBehaviour
{
    public GeneratePersonality generatePersonality;

    public int endCount = 0;
    public GameObject endCheckWindow, endWindow;

    public void OnContinueButtonClicked()
    {
        generatePersonality.c1genChart = false;
        generatePersonality.c2genChart = false;

        endCount++;
    }

    public void OnEndCheckBtnClicked()
    {
        if (endCount == 0)
        {
            endCheckWindow.SetActive(true);
        }
        else
        {
            endWindow.SetActive(true);
        }
    }
}
