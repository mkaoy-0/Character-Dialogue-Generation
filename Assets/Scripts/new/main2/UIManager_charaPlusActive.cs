using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager_charaPlusActive : MonoBehaviour
{
    public GameObject charaDataSet;
    public GameObject charaPlusButton;
    public void DontActivePlusButton()
    {
        if (charaDataSet != null && charaPlusButton != null)
        {
            if (charaDataSet.activeSelf)
            {
                if (charaPlusButton.activeSelf)
                {
                    charaPlusButton.SetActive(false);
                }
            }
        }
    }
    void Update()
    {
        DontActivePlusButton();
    }
}
