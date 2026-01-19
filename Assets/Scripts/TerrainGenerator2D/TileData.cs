using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 1種類のタイルの生成ルールを定義するデータ
/// </summary>
[CreateAssetMenu(fileName = "TileData", menuName = "Terrain/TileData")]
public class TileData : ScriptableObject
{
    /// <summary>
    /// タイルの名前
    /// </summary>
    public string Name;

    /// <summary>
    /// 実際に配置するタイル
    /// </summary>
    public TileBase Tile;

    /// <summary>
    /// 鉱石かどうか
    /// </summary>
    public bool IsOre;

    /// <summary>
    /// 生成が始まる深さ
    /// </summary>
    public int StartDepth;

    /// <summary>
    /// 生成終了深さ
    /// </summary>
    public int EndDepth;
    /// <summary>
    /// 出現確率（0.0 から 1.0）
    /// </summary>
    [Range(0f, 1f)]
    public float Probability;
}
