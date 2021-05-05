using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Monte Carlo tree search
/// and manages the visualizations
/// 
/// Node selection and search is managed here
/// as well as node creation
/// </summary>
public class MCTree : Singleton<MCTree>
{
    [SerializeField]
    private float uctTradeoff = 1f;

    [SerializeField]
    private float timeBudget = 1 / 5f;

    [SerializeField]
    private MCNode prefab;

    [SerializeField]
    private SoundListener listener;

    [SerializeField]
    private PathVisualization pathVisual; 

    private List<MCNode> nodes;
    private List<MCNode> nodesWithActions;

    private MCNode root;

    private List<Point> currentPath;

    private float stepTime;

    protected override void Awake()
    {
        base.Awake();
        nodes = new List<MCNode>();
        nodesWithActions = new List<MCNode>();
    }

    private void Start()
    {
        Setup();
    }

    public void Search()
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < timeBudget)
        {
            if (nodesWithActions.Count <= 0)
            {
                return;
            }
            Simulate();
        }

        currentPath = root.Traverse(new List<Point>());
        pathVisual.DrawPath(currentPath, TerrainContainer.instance.GetGrid());
    }

    public Queue<Point> GetPath()
    {
        return new Queue<Point>(currentPath);
    }

    private void Setup()
    {
        Point robotPos = listener.GetPos();
        root = CreateNode(null, this.transform, Direction.ROOT, 1);
        root.InitializeState(GenerateInitialState(robotPos, listener.transform.localEulerAngles.y), robotPos);
    }

    private ExtendedKalmanFilter.State GenerateInitialState(Point robotPos, float angle)
    {
        ExtendedKalmanFilter.State state = new ExtendedKalmanFilter.State(
            robotPos.x,
            robotPos.z,
            angle,
            robotPos.x, // No initial info, so the robot guesses its own position
            robotPos.z,
            0,
            Constants.Kalman.V_SOURCE_START,
            Constants.Kalman.W_SOURCE_START
            );
        return state;
    }

    public void ResetTree()
    {
        foreach (MCNode n in nodes)
        {
            Destroy(n.gameObject);
        }
        nodes.Clear();
        nodesWithActions.Clear();
        Setup();
    }

    private void Simulate()
    {
        MCNode node = SelectNode();//nodesWithActions.GetRandomElement();
        if (node.TryMove(stepTime)) // Adds node if found
        {
            nodesWithActions.Remove(node);
        }
    }

    private MCNode SelectNode()
    {
        float bestScore = Mathf.NegativeInfinity;
        MCNode bestNode = null;

        // Modified UCT scoring
        foreach (MCNode n in nodesWithActions)
        {
            float score = n.GetTotalReward() / n.GetNumTimesVisited();
            //Debug.Log("Initial: " + score);
            foreach (MCNode child in n.GetChildren())
            {
                score += uctTradeoff * Mathf.Sqrt(2 * Mathf.Log(n.GetNumTimesVisited()) / child.GetNumTimesVisited());
                //Debug.Log(n.GetNumTimesVisited());
                //Debug.Log(child.GetNumTimesVisited());
                //Debug.Log(score);
            }
            //Debug.Log("Final: " + score);
            if (score > bestScore)
            {
                bestScore = score;
                bestNode = n;
            }
        }
        return bestNode;
    }

    public MCNode CreateNode(MCNode parent, Transform parentTransform, Direction cameFrom, int level)
    {
        var node = Instantiate(prefab);
        node.transform.parent = parentTransform;
        nodes.Add(node);
        nodesWithActions.Add(node);
        List<Direction> directions = new List<Direction>(allDirections);
        if (cameFrom != Direction.ROOT)
        {
            directions.Remove(GetOpposite(cameFrom));
        }
        node.name = "Node " + cameFrom.ToString();
        node.SetActions(directions);
        node.Setup(level, parent);
        return node;
    }

    public void ShowVisual() { pathVisual.Show(); }
    public void HideVisual() { pathVisual.Hide(); }

    public static Point GetNextPoint(Point p, Direction d)
    {
        return new Point(p.x + xMoves[(int)d], p.z + zMoves[(int)d]);
    }

    private static int[] xMoves = new int[] { 0, 0, -1, 1 };
    private static int[] zMoves = new int[] { -1, 1, 0, 0 };

    public static readonly Direction[] allDirections = new Direction[]
    {
        Direction.DOWN,
        Direction.UP,
        Direction.LEFT,
        Direction.RIGHT,
    };

    public static Direction GetOpposite(Direction d)
    {
        switch (d)
        {
            case Direction.DOWN:
                return Direction.UP;
            case Direction.UP:
                return Direction.DOWN;
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
            default:
                return Direction.ROOT;
        }
    }

    public enum Direction
    {
        DOWN,
        UP,
        LEFT,
        RIGHT,
        ROOT,
    }
}
