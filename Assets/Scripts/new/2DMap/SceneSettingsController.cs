using UnityEngine;
using TMPro;

public class SceneSettingsController : MonoBehaviour
{
    public GameObject editPanel;
    [Header("x-, x+, y-, y+")]
    public TextMeshProUGUI[] label = new TextMeshProUGUI[4];
    public TMP_InputField[] tmp = new TMP_InputField[4];

    public SceneAxisManager axisManager;
    public GameObject axisPanel;

    void Start()
    {
        for (int i = 0; i < label.Length; i++)
        {
            label[i].text = tmp[i].text;
        }
    }

    // ボタン押したときにラベル更新
    public void OnEditButtonClicked()
    {
        /*/
        for (int i = 0; i < tmp.Length; i++)
        {
            if (string.IsNullOrEmpty(tmp[i].text))
            {
                return;
            }
            else if (label[i].text != tmp[i].text)
            {
                label[i].text = tmp[i].text;
            }
        }

        editPanel.SetActive(!editPanel.activeSelf);
        /**/

        if (!axisPanel.activeSelf) axisPanel.SetActive(true);
        else
        {
          //  if (axisManager.IsAxisPanelClose())
            {
                label[0].text = axisManager.GetOnLabels(axisManager.GetOnToggles()[0])[0];
                label[1].text = axisManager.GetOnLabels(axisManager.GetOnToggles()[0])[1];
                label[2].text = axisManager.GetOnLabels(axisManager.GetOnToggles()[1])[0];
                label[3].text = axisManager.GetOnLabels(axisManager.GetOnToggles()[1])[1];

                axisPanel.SetActive(false);
            }
        }
    }
}
