using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharaData
{
    public string id;   // 内部用の一意ID
    public string name; // 表示用
    public Dictionary<string, string> charaStatus;
    public Dictionary<string, int> radarCharStatus; // ノード値
    public string description;
    public Texture2D tex;

    public CharaData(string name)
    {
        this.id = Guid.NewGuid().ToString(); // 新規ID生成
        this.name = name;
        this.charaStatus = new Dictionary<string, string>();
        this.radarCharStatus = new Dictionary<string, int>(); // stats初期化
    }
}
