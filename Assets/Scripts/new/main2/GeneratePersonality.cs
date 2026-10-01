using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

public class GeneratePersonality : MonoBehaviour
{
    public GPTManager gpt;
    public UIManager_SceneSb uiManager;
    public GenerateChats generateChats;

    public TMP_InputField if_scene;

    public CreateIF_Personality createPersonality;

    private Dictionary<string, PersonalitySet> personalityDict = new Dictionary<string, PersonalitySet>();
    public Dictionary<string, PersonalitySet> PersonalityDict => personalityDict;

    public GameObject[] chartParent;
    public RadarChart chart;
    public CharaEditor editor;

    public Button[] resetBtn;

    public Button generateButton;
    public Button rcGenerateButton;

    public GameObject[] loadingObj = new GameObject[2];

    public GameObject openCharaEditorManualWindow;
    [SerializeField] private bool pgBtnIsClicked = false;

    public TextMeshProUGUI c1NameText, c2NameText;
    public bool c1genChart = false, c2genChart = false;

    public GameObject anotherRCWindow;

    public CSVSavingManager csvSavingManager;

    public void GenerateCharaPersonality()
    {
        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";

        string _scene = uiManager.CurrentSceneData(if_scene.text);
        CharaData chara1 = generateChats.db.GetCharaByName(generateChats.chara1Name.text);
        CharaData chara2 = generateChats.db.GetCharaByName(generateChats.chara2Name.text);
        string _chara1 = $"{chara1.name}({chara1.charaStatus["性別"]} | {chara1.charaStatus["年齢"]} | {chara1.charaStatus["詳細"]})";
        string _chara2 = $"{chara2.name}({chara2.charaStatus["性別"]} | {chara2.charaStatus["年齢"]} | {chara2.charaStatus["詳細"]})";

        // 詳細情報なし
        string userContent = $"次の2人のキャラ([{_chara1}][{_chara2}])の会話を分析し、それぞれの性格・感情傾向・会話スタイル・相手への態度・行動や反応の特徴を詳しく挙げてください。\n" +
                             $"会話：{_scene}" + "\n" +
            @"出力形式
[
  {
    ""chara"": ""キャラ1の名前"",
    ""personalities"": [
      ""性格1"",
      ""性格2"",
      ...
    ]
  },
  {
    ""chara"": ""キャラ2の名前"",
    ""personalities"": [
      ""性格1"",
      ""性格2"",
      ...
    ]
  }
]

- 出力は必ず上記の配列のみ。
-上記のJSON以外の文章、説明、コードブロック記号(```json や ```、chara:[]など) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
" +
"条件：\n" +
$"- 'キャラ1の名前'は{chara1.name}、'キャラ2の名前'は{chara2.name}をそのまま書くこと。「」などは不要。" + "\n" +
"- 各項目は箇条書きで、それぞれ5個程度書くこと。" + "\n" +
//"- 各項目は「セリフの内容・口調・反応」など具体的な会話描写を根拠に書くこと。" + "\n" +
//"（例：\n～というセリフ→～な性格 \n～な場面では～のような行動をとる \n感情を表に出しやすい　など。）" + "\n" +
"- 会話から読み取れる性格、特徴を積極的に書くこと。" + "\n" +
"- 深読み・心理的推測を積極的に行うこと。" + "\n" +
"- 既存の内容と同じようなものは禁止。" + "\n" +
$"（既存内容｜【{GetPersonalityList(chara1.name)}】【{GetPersonalityList(chara2.name)}】）";
        /**/

        /*/ 詳細情報あり(未完)
        string userContent = $"次の2人のキャラ([{_chara1}][{_chara2}])の会話を分析し、それぞれの性格・感情傾向・会話スタイル・相手への態度・行動や反応の特徴を詳しく挙げてください。\n" +
                             $"会話：{_scene}" + "\n" +
            @"出力形式
[
  {
    ""chara"": ""キャラ1の名前"",
    ""personalities"": [
      ""性格1"": ""性格1の詳細"",
      ""性格2"": ""性格2の詳細"",
      ...
    ]
  },
  {
    ""chara"": ""キャラ2の名前"",
    ""personalities"": [
      ""性格1"": ""性格1の詳細"",
      ""性格2"": ""性格2の詳細"",
      ...
    ]
  }
]

- 出力は必ず上記の配列のみ。
-上記のJSON以外の文章、説明、コードブロック記号(```json や ```、chara:[]など) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
" +
"条件：\n" +
$"- 'キャラ1の名前'は{chara1.name}、'キャラ2の名前'は{chara2.name}をそのまま書くこと。「」などは不要。" + "\n" +
"- 各キャラ5個以上書くこと。" + "\n" +
"- 「性格」は性格や行動特徴などを一言で書き、「詳細」は読み取った特徴の詳細や根拠を書くこと。" + "\n" +
//"（例：\n～というセリフ→～な性格 \n～な場面では～のような行動をとる \n感情を表に出しやすい　など。）" + "\n" +
"- 会話から読み取れる性格、特徴を積極的に書くこと。" + "\n" +
"- 深読み・心理的推測を積極的に行うこと。" + "\n" +
"- 既存の内容と同じようなものは禁止。" + "\n" +
$"（既存内容｜【{GetPersonalityList(chara1.name)}】【{GetPersonalityList(chara2.name)}】）";
        /**/


        Debug.Log("【プロンプト】: " + userContent);
        generateButton.interactable = false;

        //
        //csv
        csvSavingManager.WriteCsv("性格推定ボタンを押した");
        csvSavingManager.WriteCsv($"選択したシーン,{uiManager.CurrentSceneName}");
        csvSavingManager.WriteCsv("推定した性格");
        /**/

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            // 「生成中…」の表示を消す
            if (loadingObj[0] != null && loadingObj[1] != null)
            {
                loadingObj[0].SetActive(false);
                loadingObj[1].SetActive(false);
            }

            Debug.Log("【性格】受け取った返答: " + result);
            List<PersonalitySet> sets = GPTResponseParser.ParseResponse<List<PersonalitySet>>(result);

            if (sets != null)
            {
                AddPersonalitySet(sets); // 保存
                for (int i = 0; i < sets.Count; i++)
                {
                    var set = sets[i];
                    // csv
                    csvSavingManager.WriteCsv($"{set.chara}");
                    List<string> pForCsv = new();
                    /**/
                    foreach (var p in set.personalities)
                    {
                        string _setChara = System.Text.RegularExpressions.Regex.Replace(set.chara, @"\s", "");
                        createPersonality.InstantiateIF_Personality(i, p, _setChara); // UI作成
                        // csv
                        pForCsv.Add(p);
                        /**/
                    }
                    //
                    csvSavingManager.WriteCsv($"{string.Join(",", pForCsv)}");
                    /**/

                    resetBtn[i].interactable = true;
                }

                // マニュアル表示
                if (openCharaEditorManualWindow != null && !pgBtnIsClicked)
                {
                    openCharaEditorManualWindow.SetActive(true);
                }
                pgBtnIsClicked = true;
                /**/
            }

            //  Debug.Log("RCリクエスト");
            //  GenerateChartFromPersonality();
            generateButton.interactable = true;
        });
    }

    public void GenerateChartFromPersonality()
    {
        if (PersonalDictIsEmpty(editor.editingChara.name)) return;

        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";

        /*/
        CharaData chara1 = generateChats.db.GetCharaByName(generateChats.chara1Name.text);
        CharaData chara2 = generateChats.db.GetCharaByName(generateChats.chara2Name.text);
        string _chara1 = $"{GetPersonalityList(chara1.name)}";
        string _chara2 = $"{GetPersonalityList(chara2.name)}";
        /**/

        /*/
        string userContent = $"次の2人のキャラの性格をもとに、BigFiveの5項目を0～5の整数で評価してください。(0に近いほど値が低く、5に近いほど高い)\n" +
                             $"・{_chara1}" + "\n" + $"・{_chara2}" + "\n" +
            @"出力形式
[
  {
    ""chara"": ""キャラ1の名前"",
    ""value"": [
      ""Openness"": 0-5,
      ""Conscientiousness"": 0-5,
      ""Extraversion"": 0-5,
      ""Agreeableness"": 0-5,
      ""Neuroticism"": 0-5
    ]
  },
  {
    ""chara"": ""キャラ2の名前"",
    ""value"": [
      ""Openness"": 0-5,
      ""Conscientiousness"": 0-5,
      ""Extraversion"": 0-5,
      ""Agreeableness"": 0-5,
      ""Neuroticism"": 0-5
    ]
  }
]

- 出力は必ず上記の配列のみ。
-上記のJSON以外の文章、説明、コードブロック記号(```json や ```、chara:[]など) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
" +
"条件：\n" +
$"- 'キャラ1の名前'は{chara1.name}、'キャラ2の名前'は{chara2.name}をそのまま書くこと。「」などは不要。";
        /**/

        string userContent = "次の設定をもとに、性格を0～5の整数値で表してください（値が大きいほどその項目の強さが高いことを意味する）\n" +
            $"{GetPersonalityList(editor.editingChara.name)}" + "\n" +
@"項目はBIG5理論に基づいた[外向性, 開放性, 誠実性, 協調性, 神経症傾向]です。

出力形式：
{
  ""外向性"": ""整数値"",
  ""開放性"": ""整数値"",
  ""誠実性"": ""整数値"",
  ""協調性"": ""整数値"",
  ""神経症傾向"": ""整数値"",
}

- 出力は有効なJSONオブジェクト1つのみ
- JSON以外の文章は出力しない。
- JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
- 項目は必ずこの5つにしてください";

        Debug.Log("【プロンプト】: " + userContent);

        /*/
        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("【RC】受け取った返答: " + result);
            var list = GPTResponseParser.ParseResponse<List<CharacterBigFive>>(result);
            if (list == null)
            {
                Debug.LogError("BigFiveのパースに失敗しました。");
                return;
            }

            int _i = 0;
            foreach (var c in list)
            {
                if (!chartParent[_i].activeSelf)
                {
                    chart[_i].ChartInitialize();
                    chartParent[_i].SetActive(true);
                }


                //  Debug.Log($"--- {c.chara} のBigFive ---");

                var dict = c.value.ToDictionary();

                // chartノード整理
                if (dict.Count > 5) chart[_i].ClearAllNodes();

                List<int> _value = new List<int>();
                foreach (var pair in dict)
                {
                    if (pair.Key != "外向性" && pair.Key != "開放性" && pair.Key != "誠実性" && pair.Key != "協調性" && pair.Key != "神経症傾向")
                    {
                        chart[_i].AddNode(pair.Key);
                    }
                    _value.Add(pair.Value);
                 //   Debug.Log($"  {pair.Key}: {pair.Value}");
                }
                chart[_i].AutoSetNodeValues(_value.ToArray());
                editor.UpdateStatsFromChart(chart[_i], generateChats.db.GetCharaByName(c.chara));
                _i++;
            }
            Debug.Log("【RC】保存完了");
        });
        /**/

        rcGenerateButton.interactable = false;

        // csv
        csvSavingManager.WriteCsv("BigFive生成ボタンを押した");
        csvSavingManager.WriteCsv(editor.editingChara.name);
        /**/

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("【レーダーチャート】受け取った返答: " + result);
            var dict = GPTResponseParser.ParseResponse<Dictionary<string, int>>(result);
            if (dict != null)
            {
                // 追加してたラベルがあればリセット
              //  if (dict.Count > 5) chart.ClearAllNodes();

                List<int> _value = new List<int>();
                foreach (var pair in dict)
                {
                    if (pair.Key != "外向性" && pair.Key != "開放性" && pair.Key != "誠実性" && pair.Key != "協調性" && pair.Key != "神経症傾向")
                    {
                        chart.AddNode(pair.Key);
                    }
                    _value.Add(pair.Value);

                    // csv
                    csvSavingManager.WriteCsv($"{pair.Key},{pair.Value}");
                    /**/
                }

                int[] value = _value.ToArray();
                chart.AutoSetNodeValues(value);

                //
                if (editor.editingChara.name == c1NameText.text) c1genChart = true;
                else if (editor.editingChara.name == c2NameText.text) c2genChart = true;

                // チャート生成完了*2
                if (c1genChart && c2genChart)
                {
                    TextMeshProUGUI _t = anotherRCWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    // _t.text = "「×」を押して\n終了するか続けるか選択して下さい。";
                    _t.text = "このような感じで自由にシステムを操作してキャラクターを練ってください\n十分にキャラクタ―像を把握できたと思ったらアンケートに回答してください";
                   // GameObject _b = anotherRCWindow.transform.GetChild(3).gameObject;
                   // _b.SetActive(true);
                }
                anotherRCWindow.SetActive(true);
                /**/

            }
            else
            {
                Debug.LogError("raderChart のパースに失敗しました");
            }

            rcGenerateButton.interactable = true;
        });
    }

    public void RCTextVisualizer(CharaData chara, GameObject bg)
    {
       // Debug.Log(PersonalDictIsEmpty(chara.name));
        if (PersonalDictIsEmpty(chara.name))
        {
            bg.SetActive(false);
        }
        else
        {
            bg.SetActive(true);
            TextMeshProUGUI rcText = bg.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            rcText.text = editor.CharaRadarChartText(chara);
        }
    }

    //
    public void AddPersonalitySet(List<PersonalitySet> _sets)
    {
        foreach (var set in _sets)
        {
            if (personalityDict.TryGetValue(set.chara, out var existingSet))
            {
                // 既にある → 新しい personality を追加
                foreach (var p in set.personalities)
                {
                    if (!existingSet.personalities.Contains(p)) // 重複防止
                        existingSet.personalities.Add(p);
                }
            }
            else
            {
                // 初回登録
                personalityDict[set.chara] = set;
            }
        }
    }
    /**/
    /*/ 詳細情報あり（未完）
    public void AddPersonalitySet(List<PersonalitySet> _sets)
    {
        foreach (var set in _sets)
        {
            if (personalityDict.TryGetValue(set.chara, out var existingSet))
            {
               // 既にキャラが存在 → Dictionary に追加していく
            foreach (var kv in set.personalities)
            {
                string name = kv.Key;       // 性格名
                string detail = kv.Value;   // 性格の詳細

                // すでに同じ key があるならスキップ or 上書きしたいなら別処理
                if (!existingSet.personalities.ContainsKey(name))
                {
                    existingSet.personalities.Add(name, detail);
                }
                // detail を更新したい場合はこう：
                // existingSet.personalities[name] = detail;
            }
            }
            else
            {
                // 初回登録
                personalityDict[set.chara] = set;
            }
        }
    }

    /*/
    // ×ボタンで削除
    public void RemovePersonality(string cn, string text)
    {

        if (!personalityDict.ContainsKey(cn)) return;

        if (!personalityDict.TryGetValue(cn, out var kvp))
        {
            Debug.LogWarning($"キャラ '{cn}' は辞書に存在しません。");
            return;
        }

        int beforeCount = kvp.personalities.Count;

        kvp.personalities.RemoveAll(k => k == text);

        if (kvp.personalities.Count < beforeCount)
        {
            Debug.Log($"{kvp.chara} から {text} を削除");
            // csv
            csvSavingManager.WriteCsv($"{kvp.chara},推定した性格「{text}」を削除した");
        }
        else
            Debug.LogWarning($"{kvp.chara} に {text} は見つかりませんでした。");
    }
    /**/
    /*/ ×ボタンで削除
    // 詳細情報あり（未完） 
    public void RemovePersonality(string cn, string name)
    {
        if (!personalityDict.ContainsKey(cn)) return;

        if (!personalityDict.TryGetValue(cn, out var set))
        {
            Debug.LogWarning($"キャラ '{cn}' は辞書に存在しません。");
            return;
        }

        if (set.personalities.Remove(name))  // Key を削除できたら true
        {
            Debug.Log($"{set.chara} から {name} を削除");
            // csv
            csvSavingManager.WriteCsv($"{set.chara},推定した性格「{name}」を削除した");
        }
        else
        {
            Debug.LogWarning($"{set.chara} に {name} は見つかりませんでした。");
        }
    }

    /*/
    // テキスト編集したときに辞書も更新
    public void UpdatePersonality(string cn, string oldText, TMP_InputField editImp)
    {
        if (!personalityDict.ContainsKey(cn)) return;

        if (!personalityDict.TryGetValue(cn, out var kvp))
        {
            Debug.LogWarning($"キャラ '{cn}' は辞書に存在しません。");
            return;
        }

        if (oldText == editImp.text) return;

        for (int i = 0; i < kvp.personalities.Count; i++)
        {
            if (kvp.personalities[i] == oldText)
            {
                kvp.personalities[i] = editImp.text; // ← リストの中身を直接更新
                Debug.Log($"更新: {oldText} → {editImp.text}");
                // csv
                csvSavingManager.WriteCsv($"{cn},推定した性格を編集した");
                csvSavingManager.WriteCsv($"編集前,編集後");
                csvSavingManager.WriteCsv($"{oldText},{editImp.text}");
                //
                break;
            }
        }
    }
    /**/
    /*/
    // テキスト編集したときに辞書も更新
    // 詳細情報あり（未完）
    public void UpdatePersonality(string cn, string oldName, TMP_InputField editImp)
    {
        if (!personalityDict.ContainsKey(cn)) return;

        if (!personalityDict.TryGetValue(cn, out var set))
        {
            Debug.LogWarning($"キャラ '{cn}' は辞書に存在しません。");
            return;
        }

        string newName = editImp.text;
        if (oldName == newName) return;

        // oldName が存在する場合
        if (set.personalities.TryGetValue(oldName, out string detail))
        {
            // 1. 旧 Key を削除
            set.personalities.Remove(oldName);
            // 2. 新 Key で追加
            set.personalities[newName] = detail;

            Debug.Log($"更新: {oldName} → {newName}");
            // csv
            csvSavingManager.WriteCsv($"{cn},推定した性格を編集した");
            csvSavingManager.WriteCsv($"編集前,編集後");
            csvSavingManager.WriteCsv($"{oldName},{newName}");
        }
        else
        {
            Debug.LogWarning($"{set.chara} に {oldName} は見つかりませんでした。");
        }
    }
    /**/



    public void ResetPersonality(string cn)
    {
        var kvp = personalityDict[cn];
        foreach (var k in kvp.personalities)
        {
            kvp.personalities.Remove(k);
        }
        Debug.Log($"{kvp.chara}を全削除");

        // csv
        csvSavingManager.WriteCsv($"{kvp.chara},推定した性格をリセットした");

    }



    public string GetPersonalityList(string _chara)
    {
        string res = "";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{_chara}:");

        if (personalityDict.TryGetValue(_chara, out PersonalitySet data))
        {
            foreach (var p in data.personalities)
            {
                sb.AppendLine($"[{p}]");
            }
        }
        res = sb.ToString();

        return res;
    }

    public bool PersonalDictIsEmpty(string _chara)
    {
        if (personalityDict.TryGetValue(_chara, out PersonalitySet data))
        {
            if (data.personalities.Count <= 0)
            {
                return true;
            }
        }
        if (data == null) return true;
        return false;
    }
}



[Serializable]
public class PersonalitySet
{
    public string chara;                    // キャラ名
    public List<string> personalities;      // 性格リスト
    //public Dictionary<string, string> personalities;
}



[Serializable]
public class BigFiveValue
{
    public int Openness;
    public int Conscientiousness;
    public int Extraversion;
    public int Agreeableness;
    public int Neuroticism;

    public Dictionary<string, int> ToDictionary()
    {
        return new Dictionary<string, int>
        {
            {"開放性", Openness},
            {"誠実性", Conscientiousness},
            {"外向性", Extraversion},
            {"協調性", Agreeableness},
            {"神経症傾向", Neuroticism}
        };
    }
}

[Serializable]
public class CharacterBigFive
{
    public string chara;
    public BigFiveValue value;
}
