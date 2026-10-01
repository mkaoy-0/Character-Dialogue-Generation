using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetAPIKey : MonoBehaviour
{
    public TMP_InputField apiIF;
    public Button button;
    public ChatGPTClient chatGPTClient;

    private void Start()
    {
        apiIF.text = "";
    }
    void Update()
    {
        if (apiIF.text == "")
        {
            button.interactable = false;
        } else
        {
            button.interactable = true;
        }
    }

    public void SetAPItoScript()
    {
        chatGPTClient.apiKey = apiIF.text;
        Debug.Log("APIキーをセット");
    }


}
