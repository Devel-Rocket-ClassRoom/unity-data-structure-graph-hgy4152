using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefabs;
    private GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float mountainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;

    public float monsterPercent = 0.1f;

    public Vector2 tileSize = new Vector2(16, 16);

    public Sprite[] islandSprites;
    public Sprite[] FowSprites;

    private Map map;
    public Map Map => map;
    private Camera cam;

    public PlayerMovement playerPrefab;
    public PlayerMovement player;

    private Vector3 FirstPos
    {
        get
        {
            var pos = transform.position;
            pos.x -= mapWidth * tileSize.x * 0.5f;
            pos.y += mapHeight * tileSize.y * 0.5f;
            pos.x += tileSize.x * 0.5f;
            pos.y -= tileSize.y * 0.5f;
            return pos;
        }
    }

    private GameObject prev;
    private void Awake()
    {
        cam = Camera.main;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("생성");
            ResetStage();
        }


        if (tileObjs == null || tileObjs.Length == 0) return;

        // 마우스 위치를 TileId로 변환
        int index = ScreenPosToTileId(Input.mousePosition);

        if (index >= 0 && index < tileObjs.Length)
        {
            GameObject currentTile = tileObjs[index];

            if (prev != currentTile)
            {
                if (prev != null)
                {
                    prev.GetComponent<SpriteRenderer>().color = Color.white;
                }

                currentTile.GetComponent<SpriteRenderer>().color = Color.green;
                prev = currentTile;
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(player != null)
            {
                player.MoveTo(index);
                player.isMoving = false;
            }
        }
    }
    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(
            erodePercent, 
            erodeIterations,
            lakePercent,
            treePercent,
            hillPercent,
            mountainPercent,
            townPercent,
            monsterPercent
        );

        CreatePlayer();
        CreateGrid();

    }

    private void CreatePlayer()
    {
        if(player != null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab);
        int tileId = map.startTile.id;
        
        player.StartPos(tileId);


    }

    public int range = 3;
    public void VisitCheck(int tileId)
    {
        int centerX = tileId % mapWidth;
        int centerY = tileId / mapWidth;

        for (int y = centerY - range; y <= centerY + range; y++)
        {
            if (y < 0 || y >= mapHeight) 
                continue;

            for (int x = centerX - range; x <= centerX + range; x++)
            {
                if (x < 0 || x >= mapWidth) 
                    continue;

                int targetIndex = (y * mapWidth) + x;

                if(targetIndex >= 0 && targetIndex < map.tiles.Length)
                {
                    map.tiles[targetIndex].isVisited = true;
                    if (tileObjs != null)
                    {
                        DecorateTile(targetIndex);
                    }
                }


            }
        }

        // visit 경계선 변경
        int radius = range + 1;
        for (int y = centerY - radius; y <= centerY + radius; y++)
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                if (y == centerY - radius || y == centerY + radius || x == centerX - radius || x == centerX + radius)
                {
                    int targetIndex = (y * mapWidth) + x;

                    if (targetIndex >= 0 && targetIndex < map.tiles.Length)
                    {
                        map.tiles[targetIndex].UpdateFowTileId();
                        if (tileObjs != null)
                        {
                            DecorateTile(targetIndex);
                        }
                    }
                }
            }
        }
    }





    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tileObj in tileObjs)
            {
                Destroy(tileObj.gameObject);
            }
        }

        tileObjs = new GameObject[mapWidth * mapHeight];

        var position = FirstPos; // start position
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;

                var newGo = Instantiate(tilePrefabs, transform);
                tileObjs[tileId] = newGo;

                newGo.transform.position = position;
                position.x += tileSize.x;
                
                DecorateTile(tileId);
            }

            position.x = FirstPos.x;
            position.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];

        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.isVisited)
        {
            // 방문할 때 주변 미 방문 지역은 안개처리 해볼려했는데.. 한번 해보기
            if (tile.autoTileId != (int)TileTypes.Empty)
            {
                ren.sprite = islandSprites[tile.autoTileId];
            }
            else
            {
                ren.sprite = null; 
            }
        }
        else
        {
            ren.sprite = FowSprites[tile.fowTileId];
        }
    }

    // 유닛 크기를 생각 못해서 계속 꼬였음.
    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {

        var first = FirstPos;

        int x = Mathf.FloorToInt(((worldPos.x - FirstPos.x) / tileSize.x) + 0.5f);
        int y = Mathf.FloorToInt(((FirstPos.y - worldPos.y) / tileSize.y) + 0.5f);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }
    public Vector3 GetTilePos(int y, int x)
    {
        return FirstPos + new Vector3(x * tileSize.x, -y * tileSize.y);
    }
    public Vector3 GetTilePos(int tileId)
    {
        int y = tileId / mapWidth;
        int x = tileId % mapWidth;
        return GetTilePos(y, x);
    }
}
