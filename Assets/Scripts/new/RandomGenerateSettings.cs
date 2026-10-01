using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RandomGenerateSettings : MonoBehaviour
{
    public GPTManager gpt;
    public CharaEditor editor;

    public TMP_InputField if_worldSetting;

    public CharaStatusTextManager statusInp;

    public Button b_world, b_chara, b_rc;

    public RadarChart chart;

    public CSVSavingManager csvSavingManager;


    public void RandomGenerate_worldSetting()
    {
        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";
        string userContent = "物語のジャンル、舞台、世界観設定を簡単に考えてください。余計な説明は不要。簡潔に書いてください。" + "\n" +
                             $"次のジャンルと被らないようにしてください：[{if_worldSetting.text}]" + "\n" +
                           //  "また、「ファンタジー」と「サイエンスフィクション」に偏った往復をしないようにしてください。" +
                             "様々な方向性（恋愛、日常、ファンタジーなど）を積極的に検討してください。\n" + 
@"出力形式:
{
  ""genre"": ""ジャンル"",
  ""stage"": ""舞台"",
  ""worldView"": ""世界観""
}

- 出力は有効なJSONオブジェクト1つのみ
- JSON以外の文章は出力しない
- JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。";

        b_world.interactable = false;

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("【世界観】受け取った返答: " + result);
            WorldVeiwSetting _setting = GPTResponseParser.ParseResponse<WorldVeiwSetting>(result);
            if (_setting != null)
            {
                string _settingText = $"ジャンル：{_setting.genre}\n舞台：{_setting.stage}\n世界観：{_setting.worldView}";
                if_worldSetting.text = _settingText;

                // csv
                csvSavingManager.WriteCsv("世界観自動生成ボタンを押した");
                csvSavingManager.WriteCsv($"{_setting.genre},{_setting.stage},{_setting.worldView}");
                /**/
            }
            else
            {
                Debug.LogError("WorldVeiwSetting のパースに失敗しました");
            }

            b_world.interactable = true;
        });
    }


    public void RandomGenerate_charaSetting()
    {
        string worldSetting = if_worldSetting.text;
        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";

        List<CharaData> allCharas = CharaDatabase.Instance.GetAllCharas();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var chara in allCharas)
        {
            sb.AppendLine($"[{chara.name}]");
            sb.AppendLine($"{editor.CharaStatusText(chara)}");
        }
        string makedChara = sb.ToString();


        string userContent = $"次の世界観の物語に登場するキャラを1人考えてください。世界観：[{worldSetting}]" + "\n" +
                             $"次の既存キャラと異なる、様々な性格や設定のキャラを積極的に検討してください。既存キャラ：[{makedChara}]" + "\n" +
@"出力形式:
{
  ""name"": ""名前"",
  ""gender"": ""性別"",
  ""age"": ""年齢"",
  ""voice"": ""セリフ例"",
  ""detail"": ""詳細""
}

- 出力は有効なJSONオブジェクト1つのみ
- JSON以外の文章は出力しない
- JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。

条件：
 - 余計な説明は不要。簡潔に答えること。
 - ageは半角数字のみ。「歳」などの単位は不要。
 - voiceはキャラが言いそうなセリフの例や語尾などの口調を書くこと。
 - detailは立場や特徴、性格など、キャラに関する情報を簡単に書くこと。";

        b_chara.interactable = false;

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("【キャラ設定】受け取った返答: " + result);
            RandomCharaSetting _charaSetting = GPTResponseParser.ParseResponse<RandomCharaSetting>(result);
            if (_charaSetting != null)
            {
                statusInp.nameField.text = _charaSetting.name;
                statusInp.tmp_if[0].text = _charaSetting.gender;
                statusInp.tmp_if[1].text = _charaSetting.age;
                statusInp.tmp_if[2].text = _charaSetting.voice;
                statusInp.tmp_if[3].text = _charaSetting.detail;
            }
            else
            {
                Debug.LogError("RandomCharaSetting のパースに失敗しました");
            }

            b_chara.interactable = true;

        });
    }


    public void RandomGenerate_raderChart()
    {
        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";

        string _voice = (string.IsNullOrEmpty(statusInp.tmp_if[2].text)) ? "" : $"キャラの口調: [{statusInp.tmp_if[2].text}]";
        string _detail = (string.IsNullOrEmpty(statusInp.tmp_if[3].text)) ? "" : $"キャラ設定: [{statusInp.tmp_if[3].text}]";
        string charaSetting = $"{_voice} | {_detail}";

        string userContent = $"{charaSetting}" + "\n" +
@"キャラの性格を0～5の整数値で表してください（値が大きいほどその項目の強さが高いことを意味する）。
項目はBIG5理論に基づいた[外向性, 開放性, 誠実性, 協調性, 神経症傾向]です。
項目を追加しても良い。

出力形式：
{
  ""外向性"": ""整数値"",
  ""開放性"": ""整数値"",
  ""誠実性"": ""整数値"",
  ""協調性"": ""整数値"",
  ""神経症傾向"": ""整数値"",
  ...
}

- 出力は有効なJSONオブジェクト1つのみ
- JSON以外の文章は出力しない。
- JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。";

        //
        b_rc.interactable = false;

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("【レーダーチャート】受け取った返答: " + result);
            var dict = GPTResponseParser.ParseResponse<Dictionary<string, int>>(result);
            if (dict != null)
            {
                // 追加してたラベルがあればリセット
                if (dict.Count > 5) chart.ClearAllNodes();

                List<int> _value = new List<int>();
                foreach(var pair in dict)
                {
                    if (pair.Key != "外向性" && pair.Key != "開放性" && pair.Key != "誠実性" && pair.Key != "協調性" && pair.Key != "神経症傾向")
                    {
                        chart.AddNode(pair.Key);
                    }
                    _value.Add(pair.Value);
                }

                int[] value = _value.ToArray();
                chart.AutoSetNodeValues(value);
            }
            else
            {
                Debug.LogError("raderChart のパースに失敗しました");
            }

            b_rc.interactable = true;
        });
        /**/
        /*/
        chart.AddNode("AAAAA");
        int[] val = { 1, 2, 3, 4, 5, 4 };
        chart.AutoSetNodeValues(val);
        /**/
    }

}


[Serializable] 
public class WorldVeiwSetting
{
    public string genre;
    public string stage;
    public string worldView;
}

[Serializable]
public class RandomCharaSetting
{
    public string name;
    public string gender;
    public string age;
    public string voice;
    public string detail;
}