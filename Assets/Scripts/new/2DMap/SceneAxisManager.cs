using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SceneAxisManager : MonoBehaviour
{
    [SerializeField] private List<Toggle> toggles;
    private int maxSelected = 2;

    private TMP_InputField[] input5, input6;
    

    void Start()
    {
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener((_) => OnToggleChanged(toggle));
        }
        input5 = toggles[toggles.Count - 2].GetComponentsInChildren<TMP_InputField>(false);
        input6 = toggles[toggles.Count - 1].GetComponentsInChildren<TMP_InputField>(false);

    }

    // 今ONになっている2個を取得
    public Toggle[] GetOnToggles()
    {
        return toggles.Where(t => t.isOn).ToArray();
    }

    // 選択しているトグルのラベルを取得
    public string[] GetOnLabels(Toggle tog)
    {
        List<string> res = new List<string>();
        Transform p = tog.transform.GetChild(1);
        TextMeshProUGUI[] TMPLabels = p.GetComponentsInChildren<TextMeshProUGUI>(false).Where(t => t.gameObject != p.gameObject).ToArray();
        TMP_InputField[] InputLabels = p.GetComponentsInChildren<TMP_InputField>(false).Where(t => t.gameObject != p.gameObject).ToArray();
        if (InputLabels.Length > 0)
        {
            foreach(var t in InputLabels)
            {
                res.Add(t.text);
             //   Debug.Log(t.text);
                Debug.Log(t.name + "!!!");
            }
        }
        else if (TMPLabels.Length > 0)
        {
            foreach (var t in TMPLabels)
            {
                res.Add(t.text);
                Debug.Log("TMP!");
            }
        }

        Debug.Log(res.Count);
        return res.Count > 0 ? res.ToArray() : null;
    }

    // 二つ以上は選択できないようにする
    private void OnToggleChanged(Toggle changedToggle)
    {
        int onCount = toggles.Count(t => t.isOn);

        // すでに上限超えていて、今ONにしようとした場合だけOFFに戻す
        if (onCount > maxSelected && changedToggle.isOn)
        {
            changedToggle.onValueChanged.RemoveAllListeners();
            changedToggle.isOn = false;
            changedToggle.onValueChanged.AddListener((_) => OnToggleChanged(changedToggle));
        }
    }

    // パネルアクティブ切り替え可能かどうか
    public bool IsAxisPanelClose()
    {
        Toggle tog5 = toggles[toggles.Count - 2];
        Toggle tog6 = toggles[toggles.Count - 1];

        // 5だけONのとき
        if (tog5.isOn && !tog6.isOn)
        {
            return !string.IsNullOrWhiteSpace(input5[0].text) && !string.IsNullOrWhiteSpace(input5[1].text);
        }
        // 5だけONのとき
        if (tog5.isOn && !tog6.isOn)
        {
            return !string.IsNullOrWhiteSpace(input6[0].text) && !string.IsNullOrWhiteSpace(input6[1].text);
        }
        // 両方ONのとき
        if (tog5.isOn && tog6.isOn)
        {
            return !string.IsNullOrWhiteSpace(input5[0].text) && !string.IsNullOrWhiteSpace(input5[1].text) && !string.IsNullOrWhiteSpace(input6[0].text) && !string.IsNullOrWhiteSpace(input6[1].text);
        }

        // どちらもONじゃない場合
        // 2個選択されてるとき
        int onCount = toggles.Count(t => t.isOn);
        if (onCount == maxSelected)
        {
            return true;
        }
        
        // それ以外
        return false;
    }

}
