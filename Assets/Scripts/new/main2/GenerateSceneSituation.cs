using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateSceneSituation : MonoBehaviour
{
    public GPTManager gpt;

    public TMP_InputField if_worldSetting;

    public TMP_InputField if_situation;

    public Button situationButton;

    public CSVSavingManager csvSavingManager;

    public void GenerateSituation()
    {
        string worldSetting = if_worldSetting.text;
        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";

        string userContent = $"キャラ2人の会話を生成する。その会話のシーンを考えてください。\n" +
            $"[世界観：{worldSetting}]\n" +
            "大きな感情の変化やぶつかりがありそうな場面にすること。" + "\n" + 
            "出力はシーン名1つのみ。被らないような様々な場面を考えてください。\n[例：出会い、○○な会話、喧嘩、など]";

        situationButton.interactable = false;

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("受け取った返答: " + result);
            // csv
          //  csvSavingManager.WriteCsv($"シーン自動生成ボタンを押した,{result}");
            /**/
            if_situation.text = result;
            situationButton.interactable = true;
        });
    }
}
