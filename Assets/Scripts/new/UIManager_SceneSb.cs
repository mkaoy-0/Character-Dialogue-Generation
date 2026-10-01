using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UIManager_SceneSb : MonoBehaviour
{
    public GameObject sbPanel;
    public Transform content;
    public GameObject sb;
    public TextMeshProUGUI rightCharaName;

    public RectTransform sceneParent;
    public GameObject sceneButton;
    public float minDistanceX = 290f;
    public float minDistanceY = 80f;

    private List<Vector2> positions = new List<Vector2>();
    private int maxTry = 1000;

    private string _currentSceneName;
    public string CurrentSceneName
    {
        get => _currentSceneName;
        set => _currentSceneName = value;
    }

    private int _currentSceneNum;
    public int CurrentSceneNum
    {
        get => _currentSceneNum;
        set => _currentSceneNum = value;
    }


    public GameObject sceneDesBg;
    public TextMeshProUGUI sceneDes;

    // シーン吹き出しクリックしたとき
    public void PanelVisualizer()
    {
        sbPanel.SetActive(true);
    }

    // シーンボタン生成
    public void CreateSceneSb(string sceneText)
    {
        int tryCount = 0;
        while (tryCount < maxTry)
        {
            tryCount++;

            Vector2 pos = new Vector2(
                Random.Range(-275f, 275f),
                Random.Range(-460f, 460f)
            );
 
            // 既存オブジェクトと距離チェック
            bool valid = true;
            foreach (var p in positions)
            {
                if (Mathf.Abs(p.x - pos.x) < minDistanceX && Mathf.Abs(p.y - pos.y) < minDistanceY)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                GameObject obj = Instantiate(sceneButton, sceneParent);
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.localPosition = pos;

                // 子のTMPにテキストを設定
                TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = sceneText;
                }

                Button btn = obj.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    PanelVisualizer();
                    OnSceneButtonClick(sceneText);
                });
                positions.Add(pos);
                return;
            }
        }
        Debug.LogWarning("適切な位置が見つかりませんでした");
    }


    // 会話吹き出しとかを生成
    public void CreateDialogueSb_rihgt(bool isRight, string text)
    {
        GameObject obj = Instantiate(sb, content);
        Transform textObj = obj.transform.GetChild(0);
        Image objColor = obj.GetComponent<Image>();
        TextMeshProUGUI textColor = textObj.GetComponent<TextMeshProUGUI>();

        if (!isRight)
        {
            obj.transform.localScale = new Vector3(-1, 1, 1);
            textObj.transform.localScale = new Vector3(-1, 1, 1);
            objColor.color = new Color32(90, 90, 90, 255);
            textColor.color = new Color32(255, 255, 255, 255);
        }

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;       
    }

    // シーンボタン押したとき
    public void OnSceneButtonClick(string sceneText)
    {
        _currentSceneName = sceneText;

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        SceneData selected = SceneRepository.GetScene(sceneText);
        foreach (var line in selected.dialogues)
        {
            string speaker = System.Text.RegularExpressions.Regex.Replace(line.speaker, @"\s", "");
            CreateDialogueSb_rihgt(speaker == rightCharaName.text, line.text);
        }

        ActivateSceneDes(selected);
    }

    public void ActivateSceneDes(SceneData data)
    {
        if (!sceneDesBg.activeSelf) sceneDesBg.SetActive(true);
        sceneDes.text = data.sceneDescription;
    }

    // 現在選択中のシーン
    public string CurrentSceneData(string sceneName)
    {
        StringBuilder sb = new StringBuilder();

        if (!string.IsNullOrEmpty(sceneName))
        {
            sb.AppendLine($"[{sceneName}]");
        }

        SceneData selected = SceneRepository.GetScene(_currentSceneName);
        foreach (var line in selected.dialogues)
        {
            sb.AppendLine($"{line.speaker}:{line.text}");
        }

        string res = sb.ToString();

        return res;
    }

    public void sbNum(int _n)
    {
        _currentSceneNum = _n;
    }
}
