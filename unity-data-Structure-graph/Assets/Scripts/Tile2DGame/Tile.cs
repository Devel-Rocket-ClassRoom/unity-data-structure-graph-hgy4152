using UnityEngine;


public enum Sides
{
    Bottom, 
    Right,
    Left,
    Top,
}
public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;
    
    // 안개 열 때 사용
    public bool isVisited = false;

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

                autoTileId |= (1 << adjacents.Length - 1 - i);
            }
        }
    }
}
