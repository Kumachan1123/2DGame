using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDefinitions", menuName = "Scriptable Objects/TileDefinitions")]
public class TileDefinitions : ScriptableObject
{
    /// <summary>
    /// 使用するタイル定義の一覧
    /// </summary>
    public List<TileData> tileDefinitions = new List<TileData>();

    // リストを渡すためのプロパティ
    public List<TileData> TileDefinitionList => tileDefinitions;
}
