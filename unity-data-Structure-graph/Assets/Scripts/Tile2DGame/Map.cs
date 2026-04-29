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

        int total = Mathf.FloorToInt(tiles.Length * percent);

        ShuffleTiles(tiles);

        for (int i = 0; i < total; ++i)
        {
            SetWeight(tiles[i], tileType);

            if (tileType == TileTypes.Empty)
            {
                tiles[i].ClearAdjacent();
                tiles[i].weight = -1;
            }

            tiles[i].autoTileId = (int)tileType;
            
        }
    }

    private void SetWeight(Tile tile, TileTypes tileTypes)
    {
        // TileTypes. Grass(15)=1, Tree(16)=2, Hills(17)=4, Mountains(18)=MAX(통과 불가), Towns(19)=1, Castle(20)=1, Monster(21)=1.
        switch ((int)tileTypes)
        {
            case 16:
                tile.weight = 2;
                break;
            case 17:
                tile.weight = 4;
                break;
            case 18:
                tile.weight = -1;
                break;
            default:
                tile.weight = 1;
                break;
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
        for(int i = 0; i < CoastTiles.Length; i++)
        {
            CoastTiles[i].isCost = true;
        }

        for (int i = 0; i < erodeIterations; ++i)
        {
            // 해안선에 따라 구멍
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);
        }

        var landList = LandTiles; 
        if (landList.Length > 0)
        {
            ShuffleTiles(landList);
            castleTile = landList[0];
            castleTile.autoTileId = (int)TileTypes.Castle;
        }

        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);
        DecorateTiles(LandTiles, treePercent, TileTypes.Tree);
        DecorateTiles(LandTiles, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileTypes.Mountains);
        DecorateTiles(LandTiles, townPercent, TileTypes.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileTypes.Monster);

        var towns = tiles.Where(t => t.autoTileId == (int)TileTypes.Towns).ToArray();
        startTile = towns[Random.Range(0, towns.Length)];

        return true;
    }
}
