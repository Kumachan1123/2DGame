using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 森と木を生成するクラス
/// </summary>
public class ForestGenerator
{
    private Tilemap groundTilemap;
    private List<TileData> tileDefinitions;
    private int maxHeight;
    private int seed;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ForestGenerator(Tilemap tilemap, List<TileData> tileDefinitions, int maxHeight, int seed)
    {
        this.groundTilemap = tilemap;
        this.tileDefinitions = tileDefinitions;
        this.maxHeight = maxHeight;
        this.seed = seed;
    }

    /// <summary>
    /// 森を生成する
    /// </summary>
    public void Generate(int width)
    {
        TileBase trunkTile = FindTileByName("Trunk");
        TileBase leafTile = FindTileByName("Leaf");
        TileBase grassTile = FindTileByName("Grass");

        if (trunkTile == null || leafTile == null || grassTile == null)
        {
            return;
        }

        for (int x = 2; x < width - 2; x++)
        {
            bool isForest = IsForestBiome(x);

            if (!isForest)
            {
                continue;
            }

            float treeChance = 0.15f;

            if (Random.value < treeChance)
            {
                TryGenerateTreeAt(x, trunkTile, leafTile, grassTile);
            }
        }
    }

    /// <summary>
    /// 指定した列に安全に木を生成する
    /// </summary>
    private void TryGenerateTreeAt(int x, TileBase trunkTile, TileBase leafTile, TileBase grassTile)
    {
        if (!TryGetSurfaceHeight(x, out int surfaceY))
        {
            return;
        }

        Vector3Int groundPos = new Vector3Int(x, surfaceY, 0);
        TileBase groundTile = groundTilemap.GetTile(groundPos);

        /// 地表がGrassでなければ生成しない
        if (groundTile != grassTile)
        {
            return;
        }

        Vector3Int abovePos = new Vector3Int(x, surfaceY + 1, 0);

        /// 既に何かあれば生成しない
        if (groundTilemap.HasTile(abovePos))
        {
            return;
        }

        GenerateTree(x, surfaceY + 1, trunkTile, leafTile);
    }

    /// <summary>
    /// 幹と葉を生成する
    /// </summary>
    private void GenerateTree(int x, int startY, TileBase trunkTile, TileBase leafTile)
    {
        int trunkHeight = Random.Range(3, 6);

        for (int i = 0; i < trunkHeight; i++)
        {
            Vector3Int pos = new Vector3Int(x, startY + i, 0);

            if (groundTilemap.HasTile(pos))
            {
                return;
            }

            groundTilemap.SetTile(pos, trunkTile);
        }

        int leafCenterY = startY + trunkHeight;

        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                int dist = dx * dx + dy * dy;

                if (dist > 4)
                {
                    continue;
                }

                Vector3Int leafPos = new Vector3Int(x + dx, leafCenterY + dy, 0);

                if (groundTilemap.HasTile(leafPos))
                {
                    continue;
                }

                groundTilemap.SetTile(leafPos, leafTile);
            }
        }
    }

    /// <summary>
    /// 地表の高さを取得する
    /// </summary>
    private bool TryGetSurfaceHeight(int x, out int surfaceY)
    {
        for (int y = maxHeight; y >= 0; y--)
        {
            Vector3Int pos = new Vector3Int(x, y, 0);

            if (groundTilemap.HasTile(pos))
            {
                surfaceY = y;
                return true;
            }
        }

        surfaceY = 0;
        return false;
    }

    /// <summary>
    /// 指定列が森バイオームかどうか
    /// </summary>
    private bool IsForestBiome(int x)
    {
        float biomeNoise = Mathf.PerlinNoise(
            (x + seed) * 0.02f,
            0f
        );

        return biomeNoise > 0.5f;
    }

    /// <summary>
    /// TileDataのNameからタイルを取得する
    /// </summary>
    private TileBase FindTileByName(string tileName)
    {
        foreach (TileData data in tileDefinitions)
        {
            if (data == null)
            {
                continue;
            }

            if (data.Name == tileName)
            {
                return data.Tile;
            }
        }

        return null;
    }
}
