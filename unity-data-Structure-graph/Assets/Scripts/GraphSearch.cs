using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new();
    public List<GraphNode> visitNode = new();

    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode node)
    {
        path.Clear(); // 길 찾기 할 때 항상 초기화

        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        stack.Push(node);
        visited.Add(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                visited.Add(adjacent);
                stack.Push(adjacent);
            }

        }
    }
    public void BFS(GraphNode node)
    {
        path.Clear(); // 길 찾기 할 때 항상 초기화

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(node);
        visited.Add(node);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }

        }
    }



    public void DFSRecursive(GraphNode node)
    {
        path.Clear();
        DFSRecursive(node, new HashSet<GraphNode>()); // 지역변수가 매번 생성되는 것을 막을 수 있음

    }

    protected void DFSRecursive(GraphNode node, HashSet<GraphNode> visited)
    {
        path.Add(node);
        visited.Add(node);

        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || visited.Contains(adjacent))
                continue;

            DFSRecursive(adjacent, visited);

        }

    }


    public bool PathFindingBFS(GraphNode startNode, GraphNode endNode)
    {
        path.Clear(); // 길 찾기 할 때 항상 초기화

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();
        bool found = false;

        queue.Enqueue(startNode);
        visited.Add(startNode);
        startNode.previous = null;

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (currentNode == endNode)
            {
                found = true;
                break;
            }

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                visited.Add(adjacent);
                queue.Enqueue(adjacent);

                adjacent.previous = currentNode;
            }


        }

        if (found)
        {
            GraphNode temp = endNode;
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

    // 가중치가 있는 경우 대응
    // 노드 당 가중치를 가질 배열 가지고 진행
    // 우선순위 큐 사용 : 다음 노드로 넘어갈때 가장 작은 로그 반환
    public bool Dijkstra(GraphNode start, GraphNode goal)
    {
        path.Clear();
        visitNode.Clear();

        // dictionary 는 노드 id나 구분할 방법이 없을 때
        // 노드 자체로만 하기위해서 사용 하는 것
        var distances = new Dictionary<GraphNode, int>();
        var pq = new PriorityQueue<GraphNode, int>();
        var visited = new HashSet<GraphNode>();

        bool found = false;

        distances[start] = 0;
        pq.Enqueue(start, 0);
        start.previous = null;


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

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit)
                    continue;

                int newDist = distances[currentNode] + adjacent.weight;


                if (!distances.ContainsKey(adjacent) || newDist < distances[adjacent])
                {
                    distances[adjacent] = newDist;
                    adjacent.previous = currentNode;
                    pq.Enqueue(adjacent, newDist);
                }

            }

        }

        if (found)
        {
            GraphNode temp = goal;
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
    public bool AStar(GraphNode start, GraphNode goal)
    {
        path.Clear();
        visitNode.Clear();

        // dictionary 말고 int배열써서
        var distances = new int[graph.nodes.Length];
        Array.Fill(distances, int.MaxValue);


        var pq = new PriorityQueue<GraphNode, int>();
        var visited = new HashSet<GraphNode>();

        bool found = false;

        distances[start.id] = 0;

        pq.Enqueue(start, Heuristic(start, goal));
        start.previous = null;


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

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent)) 
                    continue;

                int newDist = distances[currentNode.id] + adjacent.weight;


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
            GraphNode temp = goal;
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

    private int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
}
