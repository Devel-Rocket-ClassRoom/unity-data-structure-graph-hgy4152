using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator anim;
    private bool isMoving;
    private int currentTileId;

    private Coroutine coMove;

    private List<Tile> path = new List<Tile>();
    private List<Tile> visitNode = new List<Tile>();

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0f;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }


    public void StartPos(int tileId)
    {
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }

        isMoving = false;
        currentTileId = tileId;
        transform.position = stage.GetTilePos(tileId);
        stage.VisitCheck(tileId);

    }
    public void MoveTo(int tileId)
    {
        if (!isMoving)
        {
            coMove = StartCoroutine(CoMove(tileId));

        }
    }
    public float moveSpeed = 20f;
    private IEnumerator CoMove(int tileId)
    {
        isMoving = true;

        var startTile = stage.Map.tiles[currentTileId];
        var endTile = stage.Map.tiles[tileId];

        AStar(startTile, endTile);

        anim.speed = 1f;

        if(path.Count != 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                var starPos = transform.position;
                var endPos = stage.GetTilePos(path[i].id);

                var duration = Vector3.Distance(starPos, endPos) / moveSpeed;

                var t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / duration;
                    transform.position = Vector3.Lerp(starPos, endPos, t);
                    yield return 0f;
                }

                transform.position = endPos;
                stage.VisitCheck(path[i].id);

                currentTileId = path[i].id;
            }

        }

        anim.speed = 0f;

        coMove = null;
        isMoving = false;
    }

    public bool AStar(Tile start, Tile goal)
    {
        // path를 여기서 만들어서 리턴해줘도 좋음

        path.Clear();
        visitNode.Clear();
        start.previous = null;


        bool isGoalImpassable = (goal.weight == -1);

        // dictionary 말고 int배열써서
        var distances = new int[stage.Map.tiles.Length];
        Array.Fill(distances, int.MaxValue);


        var pq = new PriorityQueue<Tile, int>();
        var visited = new HashSet<Tile>();

        bool found = false;

        distances[start.id] = 0;

        pq.Enqueue(start, Heuristic(start, goal));


        while (pq.Count > 0)
        {
            var currentNode = pq.Dequeue();

            if (visited.Contains(currentNode))
                continue;

            visited.Add(currentNode);
            visitNode.Add(currentNode);

            if (currentNode == goal)
            {
                found = true;
                break;
            }

            if (isGoalImpassable && IsAdjacent(currentNode, goal))
            {
                found = true;
                goal = currentNode;
                break;
            }

            foreach (var adjacent in currentNode.adjacents)
            {
                if (adjacent == null || visited.Contains(adjacent) || adjacent.weight == -1) continue;



                int newDist = distances[currentNode.id] + adjacent.weight;  // 가중치 더하기


                if (newDist < distances[adjacent.id])
                {
                    distances[adjacent.id] = newDist;
                    adjacent.previous = currentNode;
                    pq.Enqueue(adjacent, newDist + Heuristic(adjacent, goal));
                }

            }

        }

        if (found)
        {
            Tile temp = goal;
            while (temp != null)
            {
                path.Add(temp);
                temp = temp.previous;
            }
            path.Reverse();

            return true;

        }


        return false;
    }

    private bool IsAdjacent(Tile currentNode, Tile goal)
    {
        foreach (var adj in currentNode.adjacents)
        {
            if (adj == goal) return true;
        }
        return false;
    }

    private int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % stage.mapWidth;
        int ay = a.id / stage.mapWidth;

        int bx = b.id % stage.mapWidth;
        int by = b.id / stage.mapWidth;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

}
