using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateIF_Personality : MonoBehaviour
{
    public GameObject if_personalityPrefab;
    public Transform[] content = new Transform[2];

    public GeneratePersonality generatePersonality;
    public GenerateChats generateChats;

    public Transform content_editor;

    public void InstantiateIF_Personality(int _num, string _t, string _charaName)
    {
        GameObject obj = Instantiate(if_personalityPrefab, content[_num]);

        TMP_InputField personalText = obj.GetComponent<TMP_InputField>();
        personalText.text = _t;
        string oldText = _t;
        personalText.onEndEdit.AddListener(text =>
        {
            generatePersonality.UpdatePersonality(_charaName, oldText, personalText);
            oldText = personalText.text;
        });

        Button btn = obj.transform.GetChild(1).GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            Destroy(obj);
            generatePersonality.RemovePersonality(_charaName, personalText.text);
        });
    }

    public void InstantiateIF_PersonalityOnEditor(string _charaName)
    {
        AllClear(content_editor);

        if (!generatePersonality.PersonalityDict.TryGetValue(_charaName, out PersonalitySet kvp))
        {
           // Debug.LogWarning($"キャラ '{_charaName}' のデータが存在しません。");
            return; // 処理中断
        }
       //  var kvp = generatePersonality.PersonalityDict[_charaName];
        foreach(var k in kvp.personalities)
        {
            GameObject obj = Instantiate(if_personalityPrefab, content_editor);
            TMP_InputField personalText = obj.GetComponent<TMP_InputField>();
            personalText.text = k;
            string oldText = k;
            personalText.onEndEdit.AddListener(text =>
            {
                generatePersonality.UpdatePersonality(_charaName, oldText, personalText);
                oldText = personalText.text;
            });
            Button btn = obj.transform.GetChild(1).GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                Destroy(obj);
                generatePersonality.RemovePersonality(_charaName, k);
            });
        }
        
    }

    public void ReloadPersonalityUI(string cName)
    {
        int num;
        if (cName == generateChats.chara1Name.text) num = 0;
        else if (cName == generateChats.chara2Name.text) num = 1;
        else return;

        // まず既存UIを全部消す
        AllClear(content[num]);

        // 辞書の中身から再生成
        if (generatePersonality.PersonalityDict.TryGetValue(cName, out var data))
        {
            foreach (string p in data.personalities)
                InstantiateIF_Personality(num, p, cName);
        }
    }


    public void AllClear(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public void ResetUIAndList(int num)
    {
        for (int i = content[num].childCount - 1; i >= 0; i--)
        {
            Transform obj = content[num].GetChild(i);
            Button btn = obj.GetComponentInChildren<Button>();
            btn.onClick.Invoke();
        }
    }
} 
