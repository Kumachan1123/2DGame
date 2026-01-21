using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// タイル設置を管理するクラス
/// </summary>
public class PlacementSystem
{
    /// <summary>
    /// 設置対象のTilemap
    /// </summary>
    private Tilemap tilemap;

    /// <summary>
    /// プレイヤーのインベントリ
    /// </summary>
    private PlayerInventory inventory;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlacementSystem(Tilemap map, PlayerInventory playerInventory)
    {
        tilemap = map;
        inventory = playerInventory;
    }

    /// <summary>
    /// 指定セルにタイルを設置
    /// </summary>
    public bool Place(Vector3Int cellPos, TileData tileData)
    {
        // 持っているタイルがない場合は失敗
        if (tileData == null)
        {
            Debug.Log("Place Failed: No TileData");
            return false;
        }

        // 空気チェック：すでにタイルがある場合は失敗
        if (tilemap.HasTile(cellPos))
        {
            Debug.Log($"Place Failed: Already occupied {cellPos}");
            return false;
        }

        // 支えチェック：上下左右にタイルがなければ設置不可
        if (!HasAdjacentTile(cellPos))
        {
            Debug.Log($"Place Failed: No adjacent support {cellPos}");
            return false;
        }

        // 設置
        tilemap.SetTile(cellPos, tileData.Tile);

        // Inventory のスタックを減らす
        RemoveItemFromInventory(tileData, 1);

        Debug.Log($"Place Success: {cellPos}");
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
            if (tilemap.HasTile(checkPos))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Inventoryから指定タイルを減らす
    /// </summary>
    private void RemoveItemFromInventory(TileData tile, int amount)
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            InventorySlot slot = inventory.slots[i];

            if (slot.tileData == tile)
            {
                slot.stackCount -= amount;

                if (slot.stackCount <= 0)
                {
                    slot.tileData = null;
                    slot.stackCount = 0;
                    inventory.inventoryUI.RefreshAll();
                }

                return;
            }
        }
    }
}
