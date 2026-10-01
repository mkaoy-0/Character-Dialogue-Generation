using System.Collections.Generic;
using UnityEngine;

public class GPTManager : MonoBehaviour
{
    [Header("ChatGPT Client")]
    public ChatGPTClient chatGPTClient;

    public void RequestForGPT(string _systemContent, string _userContent, System.Action<string> onResult)
    {
        List<ChatGPTClient.MessageModel> messages = new()
        {
            new ChatGPTClient.MessageModel { role = "system", content = _systemContent },
            new ChatGPTClient.MessageModel { role = "user", content = _userContent }
        };
        Debug.Log("リクエスト送信");

        chatGPTClient.GetResponse(messages, (response) =>
        {
           // Debug.Log("レスポンス受信: " + response);
            onResult?.Invoke(response);
        });
    }

}
