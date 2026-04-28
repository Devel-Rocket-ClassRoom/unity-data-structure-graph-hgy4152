using System.Linq;
using UnityEngine;


public enum TileTypes
{
    Empty = -1,
    // 0 ~ 14 해안선
    // 땅
    Grass = 15,
    Tree,
    Hills,
    Mountains,
    Towns,
    Castle,
    Monster,
}
public class Map
{
    public int rows = 0;
    public int cols = 0;


    public Tile[] tiles;
    public Tile[] CoastTiles => tiles.Where(t => t.autoTileId >= 0 && t.autoTileId < (int)TileTypes.Grass).ToArray();
    public Tile[] LandTiles => tiles.Where(t => t.autoTileId == (int)TileTypes.Grass).ToArray();

    public Tile startTile;
    public Tile castleTile;
    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;

                // bottom
                if(r+1 < rows)
                {
                    tiles[index].adjacents[(int)Sides.Bottom] = tiles[index + cols];
                }

                // left
                if (c - 1 >= 0)
                {
                    tiles[index].adjacents[(int)Sides.Left] = tiles[index - 1];
                }

                // right
                if (c + 1 < cols)
                {
                    tiles[index].adjacents[(int)Sides.Right] = tiles[index + 1];
                }

                // top
                if(r - 1 >= 0)
                {
                    tiles[index].adjacents[(int)Sides.Top] = tiles[index - cols];
                }

            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
            tiles[i].UpdateFowTileId();

        }

    }

    public void ShuffleTiles(Tile[] tiles)
    {
        for(int i = tiles.Length - 1; i > 0; --i)
        {
            int rand = Random.Range(0, i + 1);
            (tiles[rand], tiles[i]) = (tiles[i], tiles[rand]);
        }
    }

    public void DecorateTiles(Tile[] tiles, float percent, TileTypes tileType)
    {
        ShuffleTiles(tiles);

        int total = Mathf.FloorToInt(tiles.Length * percent);

        if(TileTypes.Castle == tileType)
        {
            tiles[total/2].autoTileId = (int)tileType;
            return;
        }

        for (int i = 0; i < total; ++i)
        {
            if (tileType == TileTypes.Empty)
            {
                tiles[i].ClearAdjacent();
            }

            tiles[i].autoTileId = (int)tileType;
        }
    }


    public bool CreateIsland(
        float erodePercent,
        int erodeIterations,
        float lakePercent,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPercent
        )
    {


        for(int i = 0; i < erodeIterations; ++i)
        {
            // 해안선에 따라 구멍
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);
        }

        DecorateTiles(LandTiles, 1, TileTypes.Castle);

        // 앞에서 너무 많이 뽑아쓰면 나중에 지을 공간이 없을 수 있음
        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);
        DecorateTiles(LandTiles, treePercent, TileTypes.Tree);
        DecorateTiles(LandTiles, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileTypes.Mountains);
        DecorateTiles(LandTiles, townPercent, TileTypes.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileTypes.Monster);

        castleTile = tiles.Where(t => t.autoTileId == (int)TileTypes.Castle).ToArray()[0];
        var towns = tiles.Where(t => t.autoTileId == (int)TileTypes.Towns).ToArray();
        startTile = towns[Random.Range(0, towns.Length)];

        return true;
    }
}
