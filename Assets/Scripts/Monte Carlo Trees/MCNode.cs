using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MCTree;

/// <summary>
/// Node in a Monte Carlo Tree
/// 
/// Controls its own entropy, reward, and
/// kalman filter state estimation
/// 
/// Each node contains an extended kalman filter
/// that it uses to update its estimate of the state
/// and decide its entropy
/// </summary>
public class MCNode : MonoBehaviour
{
    [SerializeField]
    private int x;
    [SerializeField]
    private int z;

    [SerializeField]
    private float kalmanX;
    [SerializeField]
    private float kalmanZ;

    private int timesVisited;
    private double entropy;
    private double reward;

    [SerializeField]
    private float accumulatedReward;

    private int level;
    private MCNode parent;
    private List<MCNode> children;
    private ExtendedKalmanFilter ekf;
    private List<Direction> actions;

    private void Awake()
    {
        ekf = GetComponent<ExtendedKalmanFilter>();
        children = new List<MCNode>();
    }

    public void SetActions(List<Direction> directions)
    {
        this.actions = directions;
    }

    public void Setup(int level, MCNode parent)
    {
        this.level = level;
        this.parent = parent;
        this.timesVisited = 1;

        // Annoyingly enough these don't update in the editor unless
        // they are non-zero values
        // The values are initialized to 0 when they are first used
        this.accumulatedReward = 0.0f;
    }

    public void InitializeState(ExtendedKalmanFilter.State state, MathExtended.Matrices.Matrix predictionError, Point pos)
    {
        ekf.Initialize(state, predictionError, false);
        InitializationHelper(pos);
    }

    public void InitializeState(ExtendedKalmanFilter.State state, Point pos)
    {
        ekf.Initialize(state, false);
        InitializationHelper(pos);
    }

    private void InitializationHelper(Point pos)
    {
        x = pos.x;
        z = pos.z;
        transform.position = new Vector3(x, 0, z);
    }

    public float GetTotalReward()
    {
        return (float)(accumulatedReward + reward);
    }

    public int GetNumTimesVisited()
    {
        return timesVisited;
    }

    public List<MCNode> GetChildren()
    {
        return children;
    }

    private void CalculateEntropy()
    {
        this.entropy = 0;
        Point pos = new Point(x, z);
        foreach (Direction d in actions)
        {
            float prob = ApproximateSourceLocationProbability(MCTree.GetNextPoint(pos, d));
            this.entropy -= prob * Mathf.Log(prob);
        }
        this.reward = Mathf.Pow(Constants.MonteCarloTree.FORGETTING_FACTOR, level) * entropy;
        AccumulateReward((float)this.reward);
    }

    private static float minProb = 0.01f;
    private float ApproximateSourceLocationProbability(Point p)
    {
        Vector2 estimatedPos = ekf.GetState().GetSourcePos();
        float xProb = GetGaussianProb(p.x, estimatedPos.x, Constants.Kalman.DISTANCE_ERROR);
        float zProb = GetGaussianProb(p.z, estimatedPos.y, Constants.Kalman.DISTANCE_ERROR);
        return Mathf.Max(minProb, xProb * zProb);
    }

    private float GetGaussianProb(float val, float mean, double stdev)
    {
        double a = 1 / (stdev * Mathf.Sqrt(2 * Mathf.PI));
        float b = (val - mean);
        double c = -(1/2) * (b * b) / (stdev * stdev);
        return (float)(a * System.Math.Exp(c));
    }

    public void AccumulateReward(float childReward)
    {
        accumulatedReward += childReward;
        if (parent != null)
        {
            parent.AccumulateReward(childReward);
        }
    }

    public List<Point> Traverse(List<Point> lst)
    {
        lst.Add(new Point(this.x, this.z));
        if (children.Count > 0)
        {
            float maxScore = Mathf.NegativeInfinity;
            MCNode bestNode = null;
            foreach (MCNode n in children)
            {
                float score = n.GetTotalReward();
                if (score > maxScore)
                {
                    maxScore = score;
                    bestNode = n;
                }
            }
            return bestNode.Traverse(lst);
        }
        else
        {
            return lst;
        }
    }

    public bool TryMove(float treeTime)
    {
        //Debug.Log("Trying to move at " + name);
        while (actions.Count > 0)
        {
            Direction action = actions.GetRandomElement();
            actions.Remove(action);
            Point p = MCTree.GetNextPoint(new Point(x, z), action);

            if (TerrainContainer.instance.GetGrid().IsValidAndFreeTile(p.x, p.z))
            {
                MCNode node = MCTree.instance.CreateNode(this, this.transform, action, this.level + 1);
                node.InitializeState(ekf.GetState(), ekf.GetPredictionError(), p);
                node.Step(ObserveSound(), treeTime);
                children.Add(node);
                break;
            }
        }

        CalculateEntropy();
        timesVisited++;
        if (actions.Count <= 0) // No more simulation can be done on this node
        {
            Destroy(ekf);
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void Step(ExtendedKalmanFilter.Observation observation, float treeTime)
    {
        ekf.Step(observation, x, z, 0, treeTime);
        kalmanX = ekf.GetState().GetSourcePos().x;
        kalmanZ = ekf.GetState().GetSourcePos().y;
        if (float.IsNaN(kalmanX) || float.IsNaN(kalmanZ))
        {
            // If we have bad values, rerandomize estimation
            var oldState = ekf.GetState();
            float newX = x + Random.Range(-5, 5);
            float newZ = z + Random.Range(-5, 5);
            var state = new ExtendedKalmanFilter.State(
                x,
                z,
                0,
                newX,
                newZ,
                0,
                0,
                0);
            kalmanX = newX;
            kalmanZ = newZ;
            ekf.Initialize(state, false);
            name += " Reinitialized";
        }
    }

    private ExtendedKalmanFilter.Observation ObserveSound()
    {
        if (SoundSourceManager.instance.TryGetActiveSource(out SoundSource source))
        {
            Vector3 sourcePos = source.transform.localPosition;
            double x_s = sourcePos.x;
            double z_s = sourcePos.z;

            double xDiff = x_s - x;
            double zDiff = z_s - z;

            double realDist = System.Math.Sqrt(xDiff * xDiff + zDiff * zDiff);
            double realAngle = System.Math.Atan2(zDiff, xDiff);

            double errorMultiplier = System.Math.Min(realDist / Constants.Kalman.MAX_DISTANCE_FOR_ERROR, 1);

            return new ExtendedKalmanFilter.Observation(
                Gaussian.Sample(realDist, Constants.Kalman.DISTANCE_ERROR * errorMultiplier),
                Gaussian.Sample(realAngle, Constants.Kalman.ANGLE_ERROR * errorMultiplier));
        }
        else
        {
            throw new System.Exception("No active sound source");
        }
    }
}
