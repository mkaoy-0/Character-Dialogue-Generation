using UnityEngine;
using TMPro;
using System.Text.RegularExpressions; // 正規表現を使用

public class InputSpaceRemover : MonoBehaviour
{
    /// <summary>
    /// 名前テキストボックスで空白とかを消す
    /// </summary>

    public TMP_InputField inputField;

    void Start()
    {
        inputField.onValueChanged.AddListener(CleanInput);
    }

    void CleanInput(string text)
    {
        // 正規表現で「あらゆる空白文字（半角・全角・タブ・改行）」を特定
        // 変化があった文字列に空白が含まれているか判定
        if (Regex.IsMatch(text, @"\s"))
        {
            // 空白をすべて削除した文字列を作成
            string cleanedText = Regex.Replace(text, @"\s", "");

            // InputFieldに反映
            inputField.text = cleanedText;
        }
    }
}