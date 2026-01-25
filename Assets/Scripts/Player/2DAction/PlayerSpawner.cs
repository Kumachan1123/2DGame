using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// 草タイルの上にプレイヤーをスポーンさせるクラス
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    /// <summary>
    /// プレイヤーのプレハブorシーン上のプレイヤー
    /// </summary>
    [SerializeField] private GameObject m_player;
    /// <summary>
    /// 対象のタイルマップ
    /// </summary>
    [SerializeField] private Tilemap m_targetTilemap;
    /// <summary>
    /// 草タイルのタイルベース
    /// </summary>
    [SerializeField] private TileBase m_grassTileBase;
    /// <summary>
    /// プレイヤーを草タイルの上にスポーンさせる
    /// </summary>
    public void SpawnPlayerOnGrass()
    {
        List<Vector3Int> grassPositions = new List<Vector3Int>();
        // タイルマップの全てのセルをチェック
        foreach (var pos in m_targetTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = m_targetTilemap.GetTile(pos);
            if (tile == m_grassTileBase)
            {
                // 草の一マス上が空いているなら候補にする
                Vector3Int above = new Vector3Int(pos.x, pos.y + 1, pos.z);
                if (!m_targetTilemap.HasTile(above)) grassPositions.Add(above);
            }
        }
        // 草が見つからなかったら何もしない
        if (grassPositions.Count == 0)
        {
            Debug.LogWarning("No grass tiles found for player spawn.");
            return;
        }
        // ランダムに位置を選択してプレイヤーをスポーン
        Vector3Int spawnGrass = grassPositions[Random.Range(0, grassPositions.Count)];
        // ワールド座標に変換して1マス上へ
        Vector3 spawnWorldPos = m_targetTilemap.CellToWorld(spawnGrass);
        spawnWorldPos += new Vector3(0.5f, 1.5f, 0);
        // プレイヤーの位置を設定
        m_player.transform.position = spawnWorldPos;
    }


}
