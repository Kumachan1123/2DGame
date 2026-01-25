using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileData;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// TileDataを使って地表・洞窟・鉱石・森を含む2D地形を生成するクラス
/// 洞窟生成と森生成は専用クラスに委譲する
/// </summary>
public class TerrainGenerator2D : MonoBehaviour
{
    /// <summary>
    /// 地形を配置するTilemap
    /// </summary>
    public Tilemap groundTilemap;

    /// <summary>
    /// 使用するタイル定義の一覧(スクリプタブルオブジェクト）
    /// </summary>
    public TileDefinitions tileDefinitionsSO;
    /// <summary>
    /// 使用するタイル定義の一覧
    /// </summary>
    private List<TileData> tileDefinitions = new List<TileData>();

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
    /// バイオーム用ノイズの細かさ
    /// </summary>
    public float biomeNoiseScale = 0.01f;

    /// <summary>
    /// 森になるしきい値
    /// </summary>
    [Range(0f, 1f)]
    public float forestThreshold = 0.6f;

    /// <summary>
    /// シード値
    /// </summary>
    public int seed = 12345;

    /// <summary>
    /// エディタで自動生成するかどうか
    /// </summary>
    public bool generateInEditMode = true;

    /// <summary>
    /// 洞窟マップ
    /// </summary>
    private bool[,] caveMap;

    private CaveGenerator caveGenerator;
    private ForestGenerator forestGenerator;

    /// <summary>
    /// 起動時の処理
    /// </summary>
    private void Awake()
    {


    }
    private void ReadTileDefinitions()
    {
        /// タイル定義一覧を取得
        if (tileDefinitionsSO != null)
        {
            tileDefinitions = tileDefinitionsSO.TileDefinitionList;
        }
    }
    /// <summary>
    /// 開始時に地形を生成する
    /// </summary>
    private void Start()
    {
#if !UNITY_EDITOR
        GenerateTerrain();
#endif
        // プレイヤーを草地にスポーンさせる
        GetComponent<PlayerSpawner>()?.SpawnPlayerOnGrass();
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
        ReadTileDefinitions();
        GenerateTerrain();
    }

    /// <summary>
    /// 地形全体を生成する処理
    /// </summary>
    private void GenerateTerrain()
    {
        if (groundTilemap == null)
        {
            return;
        }

        groundTilemap.ClearAllTiles();

        Random.InitState(seed);

        /// 洞窟ジェネレーター生成
        caveGenerator = new CaveGenerator(width, maxHeight);

        caveMap = caveGenerator.Generate(
            seed,
            caveNoiseScale,
            caveThreshold,
            cellularIterations,
            wallThreshold
        );

        float terrainOffset = Random.Range(0f, 10000f);

        /// 地表と地中タイル配置
        for (int x = 0; x < width; x++)
        {
            float noiseValue = Mathf.PerlinNoise((x + terrainOffset) * terrainNoiseScale, 0f);
            int surfaceHeight = Mathf.FloorToInt(noiseValue * maxHeight);

            float biomeNoise = Mathf.PerlinNoise((x + terrainOffset) * biomeNoiseScale, 0f);
            bool isForest = biomeNoise > forestThreshold;

            for (int y = 0; y <= surfaceHeight; y++)
            {
                bool isSurfaceLayer = y >= surfaceHeight - 1;

                if (!isSurfaceLayer && caveMap[x, y] == false)
                {
                    continue;
                }

                TileBase tile = SelectTileForDepthAndLayer(y, surfaceHeight, isForest);

                if (tile != null)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    groundTilemap.SetTile(tilePosition, tile);
                }
            }
        }

        /// 森ジェネレーター生成
        forestGenerator = new ForestGenerator(
            groundTilemap,
            tileDefinitions,
            maxHeight,
            seed
        );

        forestGenerator.Generate(width);
    }

    /// <summary>
    /// 深さとバイオームから配置タイルを選ぶ
    /// </summary>
    private TileBase SelectTileForDepthAndLayer(int depth, int surfaceHeight, bool isForest)
    {
        int relativeDepth = surfaceHeight - depth;

        TileData selectedBase = null;

        foreach (TileData data in tileDefinitions)
        {
            if (data.IsOre)
            {
                continue;
            }

            if (data.Biome != BiomeType.Any)
            {
                if (isForest && data.Biome != BiomeType.Forest)
                {
                    continue;
                }

                if (!isForest && data.Biome != BiomeType.Plain)
                {
                    continue;
                }
            }

            if (relativeDepth < data.StartDepth || relativeDepth > data.EndDepth)
            {
                continue;
            }

            if (selectedBase == null || data.StartDepth > selectedBase.StartDepth)
            {
                selectedBase = data;
            }
        }

        bool isStoneLayer = relativeDepth >= 3;

        if (isStoneLayer)
        {
            foreach (TileData ore in tileDefinitions)
            {
                if (!ore.IsOre)
                {
                    continue;
                }

                if (ore.Biome != BiomeType.Any)
                {
                    if (isForest && ore.Biome != BiomeType.Forest)
                    {
                        continue;
                    }

                    if (!isForest && ore.Biome != BiomeType.Plain)
                    {
                        continue;
                    }
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

        if (selectedBase != null)
        {
            return selectedBase.Tile;
        }

        return null;
    }
}
