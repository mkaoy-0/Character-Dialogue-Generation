using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateNewChara : MonoBehaviour
{
    public CharaEditor editor;
    public UpdateCharaData updateCharaData;
    public GameObject charaDataSet;
    public GameObject editCanvas;
    public RawImage raw;
    public GameObject iconText;

    public CharaStatusTextManager charaStatusTextManager;
    public RadarChart chart;

    public CreateIF_Personality createIF;

    public GameObject rcTextBg;

    public GeneratePersonality generatePersonality;

    public void CreateChara()
    {
        updateCharaData.createNewChara = this;
        if (!charaDataSet.activeSelf)
        {
            editor.CreateNewChara("");
            charaStatusTextManager.SettingsInitialize();
            chart.ChartInitialize();
        }
        else
        {
          //  if (iconText.activeSelf) iconText.SetActive(false);
            editor.EditChara(charaDataSet.GetComponentInChildren<TextMeshProUGUI>().text);
            charaStatusTextManager.LoadSettings(editor.editingChara);
            chart.LoadChara(editor.editingChara);
            /*/
            if (editor.editingChara.tex != null)
            {
                raw.texture = editor.editingChara.tex;
            }
            /**/
            createIF.InstantiateIF_PersonalityOnEditor(editor.editingChara.name);
            if (generatePersonality.PersonalDictIsEmpty(editor.editingChara.name))
            {
                generatePersonality.rcGenerateButton.gameObject.SetActive(false);
            }
            else
            {
                generatePersonality.rcGenerateButton.gameObject.SetActive(true);
            }
        }
    }
}
