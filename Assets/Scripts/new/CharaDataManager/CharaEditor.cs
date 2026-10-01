using UnityEngine;
using System.Text;

public class CharaEditor : MonoBehaviour
{
    public CharaData editingChara;  // 現在編集中のキャラ

    // 新規キャラ作成
    public void CreateNewChara(string name)
    {
        editingChara = new CharaData(name);
        CharaDatabase.Instance.AddChara(editingChara);
        Debug.Log($"新規キャラ {name} を作成しました。");
    }

    // 既存キャラを編集
    public void EditChara(string name)
    {
        CharaData chara = CharaDatabase.Instance.GetCharaByName(name);
        if (chara != null)
        {
            editingChara = chara;
            Debug.Log($"キャラ {chara.name} を編集中に設定しました。");
        }
        else
        {
            Debug.LogWarning($"キャラ {name} が見つかりません。");
        }
    }


    public void UpdateCharaSettings(CharaStatusTextManager status)
    {
        UpdateName(status);
        UpdateCharaStatus(status);
    }

    // 名前更新
    public void UpdateName(CharaStatusTextManager status)
    {
        if (editingChara != null)
        {
            editingChara.name = status.nameField.text;
            Debug.Log($"編集中のキャラの名前を {status.nameField.text} に変更しました。");
        }
    }

    // キャラ情報更新
    public void UpdateCharaStatus(CharaStatusTextManager status)
    {
        if (editingChara == null) return;

        for (int i = 0; i < status.tmp_if.Count; i++)
        {
            editingChara.charaStatus[status.tmpLabel[i]] = status.tmp_if[i].text;
        }
    }


    // 値が変わったかどうか
    public bool IsStatusChanged(CharaStatusTextManager status)
    {
        if (editingChara == null) return false;

        if (status.nameField.text != editingChara.name)
        {
            Debug.Log("名前が変更された");
            return true;
        }
        int i = 0;
        foreach (var pair in editingChara.charaStatus)
        {
            if (status.tmp_if[i].text != pair.Value)
            {
                Debug.Log($"{pair.Key}が変更された");
                return true;
            }
            i++;
        }
        return false;
    }


    // ノード値更新
    public void UpdateStatsFromChart(RadarChart chart, CharaData chara = null)
    {
        if (chara != null) editingChara = chara;

        if (editingChara == null) return;

        editingChara.radarCharStatus.Clear();
        foreach (var nodeObj in chart.Nodes)
        {
            var node = nodeObj.GetComponent<RadarNode>();
            editingChara.radarCharStatus[node.label] = node.GetValue();
        }
    }

    // 値が変わったかどうか
    public bool IsChartChanged(RadarChart chart)
    {
        if (editingChara == null) return false;

        // ノード削除チェック
        foreach (var key in editingChara.radarCharStatus.Keys)
        {
            bool existsInChart = chart.Nodes.Exists(n => n.GetComponent<RadarNode>().label == key);
            if (!existsInChart)
            {
                Debug.Log($"{key} が削除された");
                return true;
            }
        }
        // ノード追加チェック
        foreach (var nodeObj in chart.Nodes)
        {
            var node = nodeObj.GetComponent<RadarNode>();
            if (!editingChara.radarCharStatus.ContainsKey(node.label))
            {
                Debug.Log($"{node.label} が新しく追加された");
                return true;
            }

            if (editingChara.radarCharStatus[node.label] != node.GetValue())
            {
                Debug.Log($"{node.label} の値が変更された");
                return true;
            }
        }
        return false;
    }

    public void PrintEditingChara()
    {
        StringBuilder sb = new StringBuilder();

        if (editingChara == null)
        {
            Debug.Log("編集中キャラはありません");
            return;
        }
       // Debug.Log($"ID: {editingChara.id}");
        Debug.Log($"キャラ名: {editingChara.name}");

        sb.AppendLine(CharaStatusText(editingChara));
        sb.AppendLine(CharaRadarChartText(editingChara));

        Debug.Log(sb.ToString());
    }

    // キャラ設定
    public string CharaStatusText(CharaData data)
    {
        if (data == null) return null;

        StringBuilder sb = new StringBuilder();
        if (data.charaStatus.Count != 0)
        {
            /*/
            foreach (var kvp in data.charaStatus)
            {
                sb.AppendLine($"・{kvp.Key}: {kvp.Value}");
            }
            /**/
            sb.AppendLine($"・性別: {data.charaStatus["性別"]}");
            sb.AppendLine($"・年齢: {data.charaStatus["年齢"]}");
            sb.AppendLine($"・口調: {data.charaStatus["口調"]}");
            sb.AppendLine($"・詳細: {data.charaStatus["詳細"]}");
        }

        return sb.ToString();
    }

    // ノード値
    public string CharaRadarChartText(CharaData data)
    {
        if (data == null) return null;

        StringBuilder sb = new StringBuilder();
        if (data.radarCharStatus.Count != 0)
        {
            foreach (var kvp in data.radarCharStatus)
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }
        }

        return sb.ToString();
    }

    // 説明文
    public void UpdateCharaDescription(CharaData data, string des)
    {
        if (data == null) return;

        data.description = des;
    }

    // 画像
    public void UpdateCharaImgTexture(CharaData data, Texture2D tex)
    {
        if (data == null) return;

        data.tex = tex;
    }
}
