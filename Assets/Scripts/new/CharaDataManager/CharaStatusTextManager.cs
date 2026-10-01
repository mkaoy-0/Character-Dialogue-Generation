using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharaStatusTextManager : MonoBehaviour
{
    public TMP_InputField nameField;
    public List<string> tmpLabel;
    public List<TMP_InputField> tmp_if;


    public void SettingsInitialize()
    {
        nameField.text = "";
        for (int i = 0; i < tmp_if.Count; i++)
        {
            tmp_if[i].text = "";
        }
    }


    public void LoadSettings(CharaData data)
    {
        nameField.text = data.name;

        int i = 0;
        foreach (var pair in data.charaStatus)
        {
            tmp_if[i].text = pair.Value;
            i++;
        }
    }

}
