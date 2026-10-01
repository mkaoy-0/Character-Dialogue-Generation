using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatePatternUI : MonoBehaviour
{
    public Button[] patternUI;
    public GameObject parent;

    public void ActivatePatternUI(string[] _text)
    {
        int _num = 0;
        foreach(var t in _text)
        {
            TextMeshProUGUI UItext = patternUI[_num].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            UItext.text = t;
         //   patternUI[_num].interactable = true;
            _num++;
        }
        parent.SetActive(true);
    }
}
