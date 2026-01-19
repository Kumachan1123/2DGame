using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// 複数種類のタイルを名前で管理して2D地形を生成するクラス
/// </summary>
public class TerrainGenerator2D : MonoBehaviour
{
    /// <summary>
    /// タイル定義用の構造体
    /// </summary>
    [System.Serializable]
    public struct TileDefinition
    {
        /// <summary>
        /// タイルの識別名
        /// </summary>
        public string name;

        /// <summary>
        /// 対応するタイル
        /// </summary>
        public TileBase tile;
    }

    /// <summary>
    /// 地形を配置するTilemap
    /// </summary>
    public Tilemap groundTilemap;

    /// <summary>
    /// 使用するタイル定義の配列
    /// </summary>
    public TileDefinition[] tileDefinitions;

    /// <summary>
    /// 横方向の生成サイズ
    /// </summary>
    public int width = 200;

    /// <summary>
    /// 最大の地面の高さ
    /// </summary>
    public int maxHeight = 20;

    /// <summary>
    /// ノイズの細かさ
    /// </summary>
    public float noiseScale = 0.05f;

    /// <summary>
    /// シード値
    /// </summary>
    public int seed = 12345;

    /// <summary>
    /// 土の層の厚み
    /// </summary>
    public int dirtLayerThickness = 3;

    /// <summary>
    /// 名前からタイルを引く辞書
    /// </summary>
    private Dictionary<string, TileBase> tileDictionary;

    /// <summary>
    /// 開始時に地形を生成する
    /// </summary>
    private void Start()
    {
        BuildTileDictionary();
        GenerateTerrain();
    }

    /// <summary>
    /// タイル定義から辞書を構築する
    /// </summary>
    private void BuildTileDictionary()
    {
        tileDictionary = new Dictionary<string, TileBase>();

        /// 定義されたタイルを辞書に登録する
        foreach (TileDefinition definition in tileDefinitions)
        {
            /// 同じ名前が登録されていないか確認する
            if (!tileDictionary.ContainsKey(definition.name))
            {
                tileDictionary.Add(definition.name, definition.tile);
            }
        }
    }

    /// <summary>
    /// 地形を生成する処理
    /// </summary>
    private void GenerateTerrain()
    {
        /// 乱数の初期化
        Random.InitState(seed);

        /// ノイズ用のオフセットをランダムに決める
        float offset = Random.Range(0f, 10000f);

        /// 横方向に順番に地形を作る
        for (int x = 0; x < width; x++)
        {
            /// ノイズ値を取得する
            float noiseValue = Mathf.PerlinNoise((x + offset) * noiseScale, 0f);

            /// ノイズ値を高さに変換する
            int height = Mathf.FloorToInt(noiseValue * maxHeight);

            /// 下からその高さまでブロックを積む
            for (int y = 0; y <= height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                /// 使用するタイル名を決める
                string tileName;

                /// 一番上は草
                if (y == height)
                {
                    tileName = "Grass";
                }
                /// 表面から数マスは土
                else if (y >= height - dirtLayerThickness)
                {
                    tileName = "Dirt";
                }
                /// それより下は岩
                else
                {
                    tileName = "Stone";
                }

                /// 辞書からタイルを取得する
                if (tileDictionary.TryGetValue(tileName, out TileBase tile))
                {
                    groundTilemap.SetTile(tilePosition, tile);
                }
            }
        }
    }
}
