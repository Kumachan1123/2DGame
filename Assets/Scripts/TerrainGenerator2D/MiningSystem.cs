using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// タイルの採掘処理を担当するクラス
/// </summary>
public class MiningSystem
{
    /// <summary>
    /// 操作対象のTilemap
    /// </summary>
    private Tilemap groundTilemap;

    /// <summary>
    /// TileDataの定義一覧
    /// </summary>
    private List<TileData> tileDefinitions;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="tilemap">地形Tilemap</param>
    /// <param name="definitions">TileData一覧</param>
    public MiningSystem(Tilemap tilemap, List<TileData> definitions)
    {
        groundTilemap = tilemap;
        tileDefinitions = definitions;
    }

    /// <summary>
    /// 指定座標のタイルを採掘できるか判定する
    /// </summary>
    /// <param name="cellPos">採掘対象のセル座標</param>
    /// <returns>採掘可能ならtrue</returns>
    public bool CanMine(Vector3Int cellPos)
    {
        /// 現在のタイルを取得
        TileBase tile = groundTilemap.GetTile(cellPos);

        /// タイルが存在しないなら不可
        if (tile == null)
        {
            return false;
        }

        /// TileData を逆引き
        TileData data = FindTileDataByTile(tile);

        /// 定義が無いなら不可
        if (data == null)
        {
            return false;
        }

        /// 採掘可能フラグを確認
        return data.IsMinable;
    }

    /// <summary>
    /// 指定座標のタイルを採掘する
    /// </summary>
    /// <param name="cellPos">採掘対象のセル座標</param>
    /// <returns>成功したらtrue</returns>
    public bool Mine(Vector3Int cellPos)
    {
        /// 採掘可能か確認
        if (!CanMine(cellPos))
        {
            return false;
        }

        /// 現在のタイルを取得
        TileBase tile = groundTilemap.GetTile(cellPos);

        /// TileData を取得
        TileData data = FindTileDataByTile(tile);

        if (data == null)
        {
            return false;
        }

        /// ドロップが設定されていれば生成
        if (data.DropPrefab != null)
        {
            Vector3 worldPos = groundTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);
            Object.Instantiate(data.DropPrefab, worldPos, Quaternion.identity);
        }

        /// Tilemap からタイルを削除
        groundTilemap.SetTile(cellPos, null);

        return true;
    }

    /// <summary>
    /// TileBase から対応する TileData を検索する
    /// </summary>
    /// <param name="tile">検索対象タイル</param>
    /// <returns>対応するTileData。無ければnull</returns>
    private TileData FindTileDataByTile(TileBase tile)
    {
        foreach (TileData data in tileDefinitions)
        {
            if (data == null)
            {
                continue;
            }

            if (data.Tile == tile)
            {
                return data;
            }
        }

        return null;
    }
}
