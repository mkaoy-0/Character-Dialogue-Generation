using UnityEngine;
using System.IO;
using System.Text;
using TMPro;

public class CSVSavingManager : MonoBehaviour
{
    public TextMeshProUGUI chara1Name, chara2Name;
    public GeneratePersonality generatePersonality;
    public CharaEditor editor;

    public TMP_InputField idField;

    public void SavingId()
    {
        string fileName = $"dataLog_{idField.text}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        using (StreamWriter writer = new StreamWriter(filePath, true, new UTF8Encoding(true))) // true = 追記
        {
            writer.WriteLine($"ID,{idField.text}");
        }
        Debug.Log("CSVにIDを保存: " + filePath);
    }


    public void WriteCsv(string str)
    {
        string fileName = $"dataLog_{idField.text}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        // ファイルに書き込む
      //  File.AppendAllText(filePath, str);
        using (StreamWriter writer = new StreamWriter(filePath, true, new UTF8Encoding(true))) // true = 追記
        {
            writer.WriteLine(str);
        }

     //   Debug.Log("CSVを書き出しました: " + filePath);

    }

    private void OnApplicationQuit()
    {
        string fileName = $"dataFinal_{idField.text}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        CharaData c1 = CharaDatabase.Instance.GetCharaByName(chara1Name.text);
        CharaData c2 = CharaDatabase.Instance.GetCharaByName(chara2Name.text);
        //   File.AppendAllText(filePath, $"{c1.name},{c1.charaStatus["性別"]},{c1.charaStatus["年齢"]},{c1.charaStatus["口調"]},{c1.charaStatus["詳細"]}");
        //  File.AppendAllText(filePath, $"{c2.name},{c2.charaStatus["性別"]},{c2.charaStatus["年齢"]},{c2.charaStatus["口調"]},{c2.charaStatus["詳細"]}");
        using (StreamWriter writer = new StreamWriter(filePath, true, new UTF8Encoding(true))) // true = 追記
        {
            writer.WriteLine($"ID,{idField.text}");
            writer.WriteLine($"{c1.name},{c1.charaStatus["性別"]},{c1.charaStatus["年齢"]},{c1.charaStatus["口調"]},{c1.charaStatus["詳細"]}");
            writer.Write($"性格,{generatePersonality.GetPersonalityList(chara1Name.text)} ");
            writer.WriteLine($"ノード値,{editor.CharaRadarChartText(c1)}");

            writer.WriteLine($"{c2.name},{c2.charaStatus["性別"]},{c2.charaStatus["年齢"]},{c2.charaStatus["口調"]},{c2.charaStatus["詳細"]}");
            writer.Write($"性格,{generatePersonality.GetPersonalityList(chara2Name.text)}");
            writer.WriteLine($"ノード値,{editor.CharaRadarChartText(c2)}");
        }

        Debug.Log("終了: " + filePath);
    }
}
