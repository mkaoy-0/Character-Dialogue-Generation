using System.Collections.Generic;
using UnityEngine;

public class CharaDatabase : MonoBehaviour
{
    public static CharaDatabase Instance;

    private void Awake() => Instance = this;

    private Dictionary<string, CharaData> charaDict = new Dictionary<string, CharaData>(); // id ÇÉLÅ[Ç…ä«óù

    public void AddChara(CharaData chara)
    {
        charaDict[chara.id] = chara;
    }

    public CharaData GetCharaByID(string id)
    {
        charaDict.TryGetValue(id, out CharaData chara);
        return chara;
    }

    public CharaData GetCharaByName(string name)
    {
        foreach (var c in charaDict.Values)
        {
            if (c.name == name) return c;
        }
        return null;
    }

    public List<CharaData> GetAllCharas() => new List<CharaData>(charaDict.Values);
}
