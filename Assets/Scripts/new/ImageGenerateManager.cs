using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

// JSONレスポンス構造
[Serializable]
public class ImgResponse
{
    public ImgData[] data;
}

[Serializable]
public class ImgData
{
    public string url;
}

public class ImageGenerateManager : MonoBehaviour
{
    public ChatGPTClient chatGPTClient;
    private string openAIApiKey;
    public CharaEditor editor;

    public ImageVisualizeManager imgVisManager;

    private void Awake()
    {
        openAIApiKey = chatGPTClient.apiKey;
    }

    public IEnumerator GenerateImg(CharaData chara, string prompt)
    {
        string url = "https://api.openai.com/v1/images/generations";
        // プロンプト受け取り確認
        Debug.Log("【画像生成】プロンプト受信: " + prompt);
        // JSON文字列を手動で構築
        string jsonBody = "{\"model\":\"dall-e-3\"," +
                          "\"prompt\":\"" + EscapeJsonComplete(prompt) + "\"," +
                          "\"n\":1," +
                          "\"size\":\"1024x1024\"," +
                          "\"quality\":\"standard\"," +   // または "hd"
                          "\"style\":\"vivid\"}";         // または "natural"
        Debug.Log("【画像生成】JSONリクエストボディ作成完了：\n" + jsonBody);

        // リクエストの作成
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + openAIApiKey);

            Debug.Log("【画像生成】リクエスト送信中…");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("【画像生成】リクエスト失敗: " + request.error);
                Debug.LogError("【画像生成】レスポンス内容: " + request.downloadHandler.text);
                yield break;
            }

            Debug.Log("【画像生成】API応答受信完了");

            // JSONレスポンスを解析
            ImgResponse response = JsonUtility.FromJson<ImgResponse>(request.downloadHandler.text);
            string imageUrl = response.data[0].url;
            Debug.Log("【画像生成】URL取得成功: " + imageUrl);

            // 画像をダウンロードしてキャラと紐づける
            yield return StartCoroutine(DownloadAndCacheImage(imageUrl, chara));
            Debug.Log("【画像生成】画像ダウンロード完了");

            // 画像表示
            imgVisManager.ShowImg();
        }
    }


    // キャッシュつきダウンロード
    IEnumerator DownloadAndCacheImage(string url, CharaData chara)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("【画像生成】画像DL失敗: " + request.error);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(request);
        editor.UpdateCharaImgTexture(chara, tex); // CharaData に保持

    }


    // JSON内で使えるように文字列をエスケープ
    public static string EscapeJsonComplete(string s)
    {
        var sb = new StringBuilder();
        foreach (char c in s)
        {
            switch (c)
            {
                case '\\': sb.Append(@"\\"); break;
                case '\"': sb.Append("\\\""); break;
                case '\b': sb.Append(@"\b"); break;
                case '\f': sb.Append(@"\f"); break;
                case '\n': sb.Append(@"\n"); break;
                case '\r': sb.Append(@"\r"); break;
                case '\t': sb.Append(@"\t"); break;
                default:
                    if (c < 32)
                    {
                        sb.Append("\\u" + ((int)c).ToString("x4"));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }
        return sb.ToString();
    }
}
