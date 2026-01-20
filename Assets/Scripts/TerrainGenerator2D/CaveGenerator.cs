using UnityEngine;

/// <summary>
/// 洞窟マップを生成するクラス
/// </summary>
public class CaveGenerator
{
    private int width;
    private int maxHeight;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CaveGenerator(int width, int maxHeight)
    {
        this.width = width;
        this.maxHeight = maxHeight;
    }

    /// <summary>
    /// 洞窟マップを生成する
    /// true = 壁
    /// false = 空洞
    /// </summary>
    public bool[,] Generate(
        int seed,
        float noiseScale,
        float threshold,
        int iterations,
        int wallThreshold)
    {
        /// 乱数初期化
        Random.InitState(seed);

        /// 初期マップ生成
        bool[,] map = InitializeMap(noiseScale, threshold);

        /// セルオートマトン反復
        for (int i = 0; i < iterations; i++)
        {
            map = DoCellularStep(map, wallThreshold);
        }

        return map;
    }

    /// <summary>
    /// 初期洞窟マップを生成する
    /// </summary>
    private bool[,] InitializeMap(float noiseScale, float threshold)
    {
        bool[,] map = new bool[width, maxHeight + 1];

        float offsetX = Random.Range(0f, 10000f);
        float offsetY = Random.Range(0f, 10000f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <= maxHeight; y++)
            {
                /// 上の方は必ず空にする
                if (y > maxHeight * 0.8f)
                {
                    map[x, y] = false;
                    continue;
                }

                float noise = Mathf.PerlinNoise(
                    (x + offsetX) * noiseScale,
                    (y + offsetY) * noiseScale
                );

                if (noise < threshold)
                {
                    map[x, y] = false;
                }
                else
                {
                    map[x, y] = true;
                }
            }
        }

        return map;
    }

    /// <summary>
    /// セルオートマトンを1ステップ進める
    /// </summary>
    private bool[,] DoCellularStep(bool[,] map, int wallThreshold)
    {
        bool[,] newMap = new bool[width, maxHeight + 1];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <= maxHeight; y++)
            {
                int wallCount = CountNeighborWalls(map, x, y);

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
                if (x == centerX && y == centerY)
                {
                    continue;
                }

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
}
