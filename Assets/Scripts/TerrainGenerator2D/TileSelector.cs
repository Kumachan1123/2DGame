using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using static TileData;

public class TileSelector
{
    private List<TileData> tileDefinitions;

    public TileSelector(List<TileData> defs)
    {
        tileDefinitions = defs;
    }

    public TileBase Select(int depth, int surfaceHeight, bool isForest)
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
