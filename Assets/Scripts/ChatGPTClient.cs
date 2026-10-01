using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPTClient : MonoBehaviour
{
    [System.Serializable]
    public class MessageModel
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class CompletionRequestModel
    {
        public string model;
        public float temperature;
        public List<MessageModel> messages;
    }

    [System.Serializable]
    public class ChatGPTRecieveModel
    {
        public Choice[] choices;

        [System.Serializable]
        public class Choice
        {
            public MessageModel message;
        }
    }

    private string _apiKey;
    public string apiKey {
        get { return _apiKey; }
        set { _apiKey = value; } 
    }

    public void GetResponse(List<MessageModel> messages, Action<string> onComplete)
    {
        StartCoroutine(Request(messages, onComplete));
    }

    private IEnumerator<UnityWebRequestAsyncOperation> Request(List<MessageModel> messages, Action<string> onComplete)
    {
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        CompletionRequestModel requestModel = new CompletionRequestModel
        {
            model = "gpt-4.1-mini",
            temperature = 1.0f,
            messages = messages
        };

        string jsonData = JsonUtility.ToJson(requestModel);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ChatGPTRecieveModel>(request.downloadHandler.text);
                string reply = response.choices[0].message.content;
                onComplete?.Invoke(reply);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

    }
}
