using UnityEngine;
using TMPro;

public class GenerateCharaDescription : MonoBehaviour
{
    public GPTManager gpt;
    public CharaEditor editor;

    public TMP_InputField if_worldSetting;

    public ImageGenerateManager imgGenerate;


    public void GenerateCharaDesctiptionByGPT(CharaData data)
    {
        if (data == null) return;
        ///
        string worldSetting = if_worldSetting.text;
        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";

        string charaStatusText = editor.CharaStatusText(data);
        string charaRCText = editor.CharaRadarChartText(data);

        /*/ // 説明文only
        string userContent = "以下の条件に従って、キャラの説明文を作成してください\n" +
                             "【キャラクター情報】\n" +
                             $"・名前: {data.name}\n" +
                             $"{charaStatusText}\n" +
                             $"・性格: \n{charaRCText}" +
                             "(性格は、各項目を0～5の数値で表したもの。値が大きいほど、その項目の強さが高いことを意味する) \n\n" +
                             $"【世界観設定】\n{worldSetting} \n\n" +
                             "【出力条件】\n" +
                             "- 文章は説明文だけにしてください。\n" +
                             "- 性別、年齢、性格、特徴、行動傾向のみを必ず文章化すること。それ以外は書かないこと。\n" +
                             "- 世界観や物語の舞台、日常のストーリー、ナレーション的要素は書かないでください。 " +
                            // "- 性格の数値は文章で表現すること。例:「○○が5段階中3なので、～～な性格。○○が5なので、」\n" +
                             "- 短すぎず、キャラクターの雰囲気がイメージできる文章にすること。\n" +
                             "- 会話生成に使いやすい文章にすること。\n" +
                             "- 小説やゲームのキャラクター紹介文のような、説明口調の自然な文体にすること。";
        /**/
        // 
        string userContent = "以下の条件に従って、キャラの説明文を作成してください\n" +
                     "【キャラクター情報】\n" +
                     $"・名前: {data.name}\n{charaStatusText}\n・性格: \n{charaRCText}" +
                     "(性格は、各項目を0～5の数値で表したもの。値が大きいほど、その項目の強さが高いことを意味する) \n\n" +
                     $"【世界観設定】\n{worldSetting} \n\n" +
                     "【出力条件】\n" +
                     "- personalityDescription: キャラ説明文。性格、特徴、行動傾向のみを必ず文章化すること。名前、性別、年齢は書かないこと。\n" +
                 //    "- appearanceDescription: 髪型、髪色、瞳の色、服装（主に上半身に見える部分）、全体の雰囲気を説明すること。\n" +
                     "- 指定した要素以外は書かないこと。世界観や物語の舞台、日常のストーリー、ナレーション的要素は書かないでください。\n" +
                     "- 説明文は短すぎず、キャラクターの雰囲気がイメージできる文章にすること。" +
                    // "- 会話生成に使いやすい文章にすること。\n" +
                     "- 小説やゲームのキャラクター紹介文のような、説明口調の自然な文体にすること。\n" +
                     "- 出力は有効なJSONオブジェクト1つのみ。\n" +
                     "- JSON以外の文章は出力しない。\n" +
                     "- JSON内の文字列は必ずダブルクォートで囲むこと。\n" +
                     "-フォーマットを絶対に崩さないでください。\n\n" + 
@"出力形式:
{
  ""personalityDescription"": ""キャラ説明文"",
}";

        //   ""appearanceDescription"": ""外見情報""



        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("受け取った返答: " + result);
            CharaDescriptionSet description = GPTResponseParser.ParseResponse<CharaDescriptionSet>(result);
            if (description != null)
            {
                // 説明文をキャラデータに保存
                editor.UpdateCharaDescription(data, description.personalityDescription);
                // 画像生成
                // とりあえず1回のみ  
                /*/
                if (data.tex == null)
                {
                    if (!string.IsNullOrEmpty(description.appearanceDescription))
                    {
                        string imgPrompt = $"年齢が{data.charaStatus["年齢"]}で性別が{data.charaStatus["性別"]}のキャラクター。\n" +
                                           description.appearanceDescription + "\n" +
                                           "- 日本アニメ風のイラスト\n- バストアップ\n- 一体のみ。差分は絶対に描かないこと" + 
                                           "- 年齢より若干若めに描いてください" + 
                                           "- 背景は単色のシンプルなもの";

                        StartCoroutine(imgGenerate.GenerateImg(data, imgPrompt));
                    }
                }
                /**/
            }
            else
            {
                Debug.LogError("CharaDescriptionSet のパースに失敗しました");
            }
        });

    }
}
