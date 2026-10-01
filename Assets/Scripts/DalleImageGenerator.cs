using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

public class DalleImageGenerator : MonoBehaviour
{
    [Header("UI Components")]
    private string openAIApiKey;  // ←ここにあなたのAPIキー
    public ChatGPTClient chatGPTClient;

    [Header("Prompt")]
    public string prompt = "anime-style illustration, 1 girl, standing, full body shot, white background, blond long hairl";

    void Start()
    {
        openAIApiKey = chatGPTClient.apiKey;
      //  StartCoroutine(GenerateImageFromPrompt(prompt));
    }

    public IEnumerator GenerateImageFromPrompt(CharacterData targetChara, string prompt)
    {
        string url = "https://api.openai.com/v1/images/generations";

        // プロンプト受け取り確認
        Debug.Log("プロンプト受信: " + prompt);

        /*/ Dall-e 2 =========================================
        // リクエストボディをJSONで組み立て
        string jsonBody = JsonUtility.ToJson(new ImageRequest(prompt));
        /**/
        // ==================================================

        // Dall-e 3 =========================================
        // JSON文字列を手動で構築
        string jsonBody = "{\"model\":\"dall-e-3\"," +
                          "\"prompt\":\"" + EscapeJsonComplete(prompt) + "\"," +
                          "\"n\":1," +
                          "\"size\":\"1024x1024\"," +
                          "\"quality\":\"standard\"," +   // または "hd"
                          "\"style\":\"vivid\"}";         // または "natural"
        /**/
        // ==================================================
        Debug.Log("JSONリクエストボディ作成完了：\n" + jsonBody);

        // リクエストの作成
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + openAIApiKey);

            Debug.Log("OpenAI APIにリクエスト送信中…");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("リクエスト失敗: " + request.error);
                Debug.LogError("レスポンス内容: " + request.downloadHandler.text);
                yield break;
            }

            Debug.Log("API応答受信完了");

            // JSONレスポンスを解析
            ImageResponse response = JsonUtility.FromJson<ImageResponse>(request.downloadHandler.text);
            string imageUrl = response.data[0].url;
            Debug.Log("画像URL取得成功: " + imageUrl);

            // 画像をダウンロードしてキャラと紐づける
            yield return StartCoroutine(DownloadAndCacheImage(imageUrl, targetChara));
            Debug.Log("画像のダウンロードが完了しました");
        }
    }

    // 画像を設定
    public void ShowCharaImage(RawImage rawImg, CharacterData targetChara)
    {
        rawImg.texture = targetChara.GetImgTex();
        rawImg.SetNativeSize();
        Debug.Log("画像を表示");
    }

    // キャッシュつきダウンロード
    IEnumerator DownloadAndCacheImage(string url, CharacterData charaData)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("画像DL失敗: " + request.error);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(request);
        charaData.SetImgTex(tex); // CharacterData に保持
    }

    // JSON内で使えるように文字列をエスケープ
    public static string EscapeJsonComplete(string s)
    {
        var sb = new System.Text.StringBuilder();
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


    /*/ Dall-e 2 ========
    // JSONリクエスト構造
    [System.Serializable]
    public class ImageRequest
    {
        public string prompt;
        public int n = 1;
        public string size = "512x512";

        public ImageRequest(string prompt)
        {
            this.prompt = prompt;
        }
    }
    /*/
    // =====================

    // JSONレスポンス構造
    [System.Serializable]
    public class ImageResponse
    {
        public ImageData[] data;
    }

    [System.Serializable]
    public class ImageData
    {
        public string url;
    }
}
