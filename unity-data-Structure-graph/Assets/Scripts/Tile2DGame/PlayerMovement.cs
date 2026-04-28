using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator anim;
    private int currentTileId;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }


    private void Update()
    {


        var direction = Sides.None;

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            
            direction = Sides.Top;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Sides.Bottom;

        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Sides.Right;

        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Sides.Left;

        }

        if(direction != Sides.None)
        {
            // 포지션을 옮기는게 아닌 Sides에 맞는 이웃한 노드로 캐릭터 이동
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];

            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }

    }

    public void MoveTo(int tileId)
    {
        currentTileId = tileId;

        transform.position = stage.GetTilePos(currentTileId);
        stage.VisitCheck(tileId);
      
    }

}
