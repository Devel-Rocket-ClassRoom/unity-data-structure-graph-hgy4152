using UnityEngine;


public enum Sides
{
    None = -1,
    Top, 
    Left,
    Right,
    Bottom,
}
public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;
    public int fowTileId;
    
    // 안개 열 때 사용
    public bool isVisited = false;
    public bool CanMove => autoTileId != (int)TileTypes.Empty;
    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                // 비트 시프트
                // 1000 0
                // 0100 1
                // 0010 2
                // 0001 3

                autoTileId |= 1 << i;
            }
        }
    }

    public void UpdateFowTileId()
    {
        fowTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                // 비트 시프트
                // 1000 0
                // 0100 1
                // 0010 2
                // 0001 3

                fowTileId |= 1 << i;
            }
        }
    }

    public void RemoveAdjacent(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }

            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                UpdateAutoTileId();

                break;
            }
        }
    }

    public void ClearAdjacent()
    {
        // 맵 구멍 낼 때

        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }

            // 이웃과 연결 해제
            adjacents[i].RemoveAdjacent(this);
            adjacents[i] = null; 
        }

        UpdateAutoTileId();

    }
}
