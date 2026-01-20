using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// タイル設置を管理するクラス
/// </summary>
public class PlacementSystem
{
    /// <summary>
    /// 設置対象のタイルマップ
    /// </summary>
    public Tilemap groundTilemap;

    public PlacementSystem(Tilemap tilemap)
    {
        groundTilemap = tilemap;
    }

    /// <summary>
    /// タイルを設置する
    /// </summary>
    public bool Place(Vector3Int cellPos, TileData tileData)
    {
        /// 何も持っていなければ失敗
        if (tileData == null)
        {
            Debug.Log("Place Failed : No TileData");
            return false;
        }

        /// 空気チェック
        if (groundTilemap.HasTile(cellPos))
        {
            Debug.Log($"Place Failed : Already occupied {cellPos}");
            return false;
        }

        /// 支えチェック（上下左右）
        if (!HasAdjacentTile(cellPos))
        {
            Debug.Log($"Place Failed : No adjacent support {cellPos}");
            return false;
        }

        /// 設置
        groundTilemap.SetTile(cellPos, tileData.Tile);

        Debug.Log($"Place Success : {cellPos}");
        return true;
    }

    /// <summary>
    /// 上下左右にタイルがあるかチェック
    /// </summary>
    private bool HasAdjacentTile(Vector3Int cellPos)
    {
        Vector3Int[] directions =
        {
            new Vector3Int( 1,  0, 0),
            new Vector3Int(-1,  0, 0),
            new Vector3Int( 0,  1, 0),
            new Vector3Int( 0, -1, 0),
        };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int checkPos = cellPos + dir;

            if (groundTilemap.HasTile(checkPos))
            {
                return true;
            }
        }

        return false;
    }
}
