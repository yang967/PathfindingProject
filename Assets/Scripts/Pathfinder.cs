using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder instance;

    bool[] cost;
    int grid;
    int width, height;
    public float height_detection_threshold { get; set; }
    public LayerMask Ignored_Layer { get; set; }

    int xLowerBound, yLowerBound, xHigherBound, yHigherBound;

    private void Awake()
    {
        instance = this;
        Ignored_Layer = 1 << 2;
        height_detection_threshold = 2;
        SetGrid(2);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGrid(int grid)
    {
        this.grid = grid;
        width = 1000 / grid;
        height = 1000 / grid;

        xLowerBound = -500;
        yLowerBound = -500;
        xHigherBound = 500 - grid;
        yHigherBound = 500 - grid;

        int length = width * height;
        cost = new bool[length];
        for(int i = 0; i < height; i++)
            for(int j = 0; j < width; j++)
                cost[i * width + j] = Detection.detect(new Vector3(i * grid - 500, height_detection_threshold, j * grid - 500), grid, Ignored_Layer);
    }

    public List<Vector3> AStar(Vector3 start, Vector3 end)
    {
        start = new Vector3(start.x > xHigherBound ? xHigherBound : start.x < xLowerBound ? xLowerBound : start.x, start.y, start.z > yHigherBound ? yHigherBound : start.z < yLowerBound ? yLowerBound : start.z);
        end = new Vector3(end.x > xHigherBound ? xHigherBound : end.x < xLowerBound ? xLowerBound : end.x, end.y, end.z > yHigherBound ? yHigherBound : end.z < yLowerBound ? yLowerBound : end.z);
        Vector3 s = Normalize(start);
        Vector3 e = Normalize(end);
        Node current = new Node(0, 0, 0, s, grid, width);
        current.h = Heuristic(current, end);
        current.f = current.h;

        MapHeap heap = new MapHeap();
        Node[] dp = new Node[width * height];
        for (int i = 0; i < height * width; i++)
        {
            dp[i] = new Node();
            dp[i].indx = i;
        }
        dp[current.indx] = current;
        heap.Push(current);

        while(!heap.isEmpty())
        {
            current = heap.Pop();

            Vector3 current_vec3 = Node.Indx2Vector3(current.indx, grid, width);
            if (current_vec3.x == e.x && current_vec3.z == e.z)
                break;
            
            List<Vector3> neighbors = SearchNearbyNode(current);

            foreach(Vector3 position in neighbors)
            {
                int indx = Node.Vector3ToIndx(position, grid, width);
                int distance = Heuristic(current, position);

                if(dp[indx].g > current.g + distance)
                {
                    dp[indx].g = current.g + distance;
                    dp[indx].h = Heuristic(dp[indx], end);
                    dp[indx].f = dp[indx].g + dp[indx].h;
                    dp[indx].prev = current.indx;
                    heap.Push(dp[indx]);
                }
            }
        }

        return ConstructPath(dp, start, end);
    }

    int Heuristic(Node s, Vector3 end)
    {
        Vector3 start = Node.Indx2Vector3(s.indx, grid, width);
        int xDiff = Mathf.RoundToInt(Mathf.Abs(start.x - end.x)) / grid;
        int yDiff = Mathf.RoundToInt(Mathf.Abs(start.z - end.z)) / grid;

        return Mathf.Min(xDiff, yDiff) * 14 + (Mathf.Max(xDiff, yDiff) - Mathf.Min(xDiff, yDiff)) * 10;
    }

    int Heuristic(Node s, Node e)
    {
        Vector3 start = new Vector3(s.indx / width * grid + 500, height_detection_threshold, s.indx % width * grid + 500);
        Vector3 end = new Vector3(e.indx / width * grid + 500, height_detection_threshold, e.indx % width * grid + 500);

        int xDiff = Mathf.RoundToInt(Mathf.Abs(start.x - end.x));
        int yDiff = Mathf.RoundToInt(Mathf.Abs(start.z - end.z));

        //return (Mathf.Max(xDiff, yDiff) - Mathf.Min(xDiff, yDiff)) * 14 + Mathf.Min(xDiff, yDiff) * 10;
        return xDiff * xDiff + yDiff * yDiff;
    }

    List<Vector3> SearchNearbyNode(Node node)
    {
        Vector3 center = Node.Indx2Vector3(node.indx, grid, width);

        List<Vector3> result = new List<Vector3>();
        int times = 0;
        for (int i = center.x - grid < xLowerBound ? xLowerBound : (int)center.x - grid; i <= (center.x + grid >= xHigherBound ? xHigherBound : (int)center.x + grid); i += grid)
            for(int j = center.z - grid < yLowerBound ? yLowerBound : (int)center.z - grid; j <= (center.z + grid >= yHigherBound ? yHigherBound : (int)center.z + grid); j += grid)
            {
                if ((i == center.x && j == center.z) || cost[Node.Vector3ToIndx(new Vector3(i, 0, j), grid, width)])
                    continue;
                result.Add(new Vector3(i, height_detection_threshold, j));
                times += 1;
            }
        return result;
    }

    List<Vector3> ConstructPath(Node[] dp, Vector3 start, Vector3 end)
    {
        int current = Node.Vector3ToIndx(end, grid, width);
        List<Vector3> path = new List<Vector3>();

        path.Add(end);
        while (dp[current].prev != dp[current].indx)
        {
            path.Add(Node.Indx2Vector3(current, grid, width));
            current = dp[current].prev;
        }
        return path;
    }

    Vector3 Normalize(Vector3 position)
    {
        return new Vector3(Mathf.RoundToInt(position.x / grid) * grid, height_detection_threshold, Mathf.RoundToInt(position.z / grid) * grid);
    }

    
}
public class Node {
    public int h;
    public int f;
    public int g;
    public int indx;
    public int prev;

    public Node()
    {
        h = 0;
        g = 65535;
        f = 65535;
        indx = -1;
        prev = -1;
    }

    public Node(int h, int f, int g, int indx)
    {
        this.h = h;
        this.f = f;
        this.g = g;
        this.indx = indx;
        prev = indx;
    }

    public Node(int h, int f, int g, Vector3 position, int grid, int width)
    {
        int indx = (int)(position.x + 500) / grid * width + (int)(position.z + 500) / grid;
        this.h = h;
        this.f = f;
        this.g = g;
        this.indx = indx;
        prev = indx;
    }

    public static int Vector3ToIndx(Vector3 position, int grid, int width)
    {
        return (int)(position.x + 500) / grid * width + (int)(position.z + 500) / grid;
    }

    public static Vector3 Indx2Vector3(int indx, int grid, int width)
    {
        return new Vector3(indx / width * grid - 500, 0, indx % width * grid - 500);
    }
}
