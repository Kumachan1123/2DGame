using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// TileDataを使って地表・洞窟・鉱石を含む2D地形を生成するクラス
/// </summary>
public class TerrainGenerator2D : MonoBehaviour
{
    /// <summary>
    /// 地形を配置するTilemap
    /// </summary>
    public Tilemap groundTilemap;

    /// <summary>
    /// 使用するタイル定義の一覧
    /// </summary>
    public List<TileData> tileDefinitions = new List<TileData>();

    /// <summary>
    /// 横方向の生成サイズ
    /// </summary>
    public int width = 200;

    /// <summary>
    /// 最大の地面の高さ
    /// </summary>
    public int maxHeight = 20;

    /// <summary>
    /// 地表用ノイズの細かさ
    /// </summary>
    public float terrainNoiseScale = 0.05f;

    /// <summary>
    /// 洞窟用ノイズの細かさ
    /// </summary>
    public float caveNoiseScale = 0.25f;

    /// <summary>
    /// 洞窟になるしきい値
    /// </summary>
    [Range(0f, 1f)]
    public float caveThreshold = 0.5f;

    /// <summary>
    /// セルオートマトンの反復回数
    /// </summary>
    public int cellularIterations = 8;

    /// <summary>
    /// 壁とみなす近傍数のしきい値
    /// </summary>
    public int wallThreshold = 4;

    /// <summary>
    /// シード値
    /// </summary>
    public int seed = 12345;

    /// <summary>
    /// エディタで自動生成するかどうか
    /// </summary>
    public bool generateInEditMode = true;

    /// <summary>
    /// 内部的に使う洞窟マップ
    /// true = 壁  false = 空洞
    /// </summary>
    private bool[,] caveMap;

    /// <summary>
    /// 開始時に地形を生成する
    /// </summary>
    private void Start()
    {
#if !UNITY_EDITOR
        GenerateTerrain();
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// インスペクター変更時に自動生成する
    /// </summary>
    private void OnValidate()
    {
        if (!Application.isPlaying && generateInEditMode)
        {
            GenerateTerrain();
        }
    }
#endif

    /// <summary>
    /// エディターから地形を生成する
    /// </summary>
    public void GenerateFromEditor()
    {
        GenerateTerrain();
    }

    /// <summary>
    /// 地形全体を生成する処理
    /// </summary>
    private void GenerateTerrain()
    {
        /// Tilemapが未設定なら何もしない
        if (groundTilemap == null)
        {
            return;
        }

        /// 既存タイルをすべて削除
        groundTilemap.ClearAllTiles();

        /// 乱数の初期化
        Random.InitState(seed);

        /// 洞窟マップを初期化
        InitializeCaveMap();

        /// セルオートマトンで洞窟を整形
        for (int i = 0; i < cellularIterations; i++)
        {
            caveMap = DoCellularStep(caveMap);
        }

        /// 地表ノイズ用オフセット
        float terrainOffset = Random.Range(0f, 10000f);

        /// 横方向に地形を生成
        for (int x = 0; x < width; x++)
        {
            /// 地表の高さを決定
            float noiseValue = Mathf.PerlinNoise((x + terrainOffset) * terrainNoiseScale, 0f);
            int surfaceHeight = Mathf.FloorToInt(noiseValue * maxHeight);

            /// 下から地表まで積む
            for (int y = 0; y <= surfaceHeight; y++)
            {
                /// 地表付近は洞窟を作らない
                bool isSurfaceLayer = y >= surfaceHeight - 1;

                /// 洞窟ならスキップ
                if (!isSurfaceLayer && caveMap[x, y] == false)
                {
                    continue;
                }

                /// 配置するタイルを決定
                TileBase tile = SelectTileForDepthAndLayer(y, surfaceHeight);

                /// タイルがあれば配置
                if (tile != null)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    groundTilemap.SetTile(tilePosition, tile);
                }
            }
        }
    }

    /// <summary>
    /// 洞窟マップの初期配置を行う
    /// </summary>
    private void InitializeCaveMap()
    {
        caveMap = new bool[width, maxHeight + 1];

        float offsetX = Random.Range(0f, 10000f);
        float offsetY = Random.Range(0f, 10000f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <= maxHeight; y++)
            {
                /// 上の方は必ず空にする
                if (y > maxHeight * 0.8f)
                {
                    caveMap[x, y] = false;
                    continue;
                }

                /// PerlinNoiseで洞窟の元を生成
                float noise = Mathf.PerlinNoise(
                    (x + offsetX) * caveNoiseScale,
                    (y + offsetY) * caveNoiseScale
                );

                /// しきい値で壁か空洞か決定
                if (noise < caveThreshold)
                {
                    caveMap[x, y] = false;
                }
                else
                {
                    caveMap[x, y] = true;
                }
            }
        }
    }

    /// <summary>
    /// セルオートマトンを1ステップ進める
    /// </summary>
    private bool[,] DoCellularStep(bool[,] map)
    {
        bool[,] newMap = new bool[width, maxHeight + 1];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <= maxHeight; y++)
            {
                int wallCount = CountNeighborWalls(map, x, y);

                /// 壁が多ければ壁にする
                if (wallCount >= wallThreshold)
                {
                    newMap[x, y] = true;
                }
                else
                {
                    newMap[x, y] = false;
                }
            }
        }

        return newMap;
    }

    /// <summary>
    /// 周囲8マスの壁の数を数える
    /// </summary>
    private int CountNeighborWalls(bool[,] map, int centerX, int centerY)
    {
        int count = 0;

        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                /// 自分自身は無視する
                if (x == centerX && y == centerY)
                {
                    continue;
                }

                /// 範囲外は壁として扱う
                if (x < 0 || x >= width || y < 0 || y > maxHeight)
                {
                    count++;
                }
                else if (map[x, y])
                {
                    count++;
                }
            }
        }

        return count;
    }

    private TileBase SelectTileForDepthAndLayer(int depth, int surfaceHeight)
    {
        /// 地表からの相対的な深さ
        int relativeDepth = surfaceHeight - depth;

        TileData selectedBase = null;

        /// 通常ブロックを選択
        foreach (TileData data in tileDefinitions)
        {
            /// 鉱石は無視
            if (data.IsOre)
            {
                continue;
            }

            /// 相対深さ範囲外なら無視
            if (relativeDepth < data.StartDepth || relativeDepth > data.EndDepth)
            {
                continue;
            }

            /// より深い層用のものを優先する
            if (selectedBase == null || data.StartDepth > selectedBase.StartDepth)
            {
                selectedBase = data;
            }
        }

        /// 石層なら鉱石抽選
        bool isStoneLayer = relativeDepth >= 3;

        if (isStoneLayer)
        {
            foreach (TileData ore in tileDefinitions)
            {
                if (!ore.IsOre)
                {
                    continue;
                }

                if (relativeDepth < ore.StartDepth || relativeDepth > ore.EndDepth)
                {
                    continue;
                }

                float r = Random.value;

                if (r < ore.Probability)
                {
                    return ore.Tile;
                }
            }
        }

        /// 通常ブロックを返す
        if (selectedBase != null)
        {
            return selectedBase.Tile;
        }

        return null;
    }
}
