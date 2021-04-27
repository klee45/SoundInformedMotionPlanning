using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundListener : MonoBehaviour
{
    [SerializeField]
    private Timer stepTimer;
    [SerializeField]
    private Timer moveTimer;
    [SerializeField]
    private float timeBeforePlanning;

    [SerializeField]
    private NoisyGrid grid;
    [SerializeField]
    private EstimateVisual estimate;

    private Queue<Point> currentPath;

    private int x;
    private int z;

    private Vector3 anglesAtMoveStart;
    private Vector3 positionAtMoveStart;
    private float timeAtMoveStart;

    private ExtendedKalmanFilter ekf;

    private void Awake()
    {
        this.ekf = new ExtendedKalmanFilter();
    }

    private void Start()
    {
        Vector3 pos = transform.localPosition;
        Vector3 sourcePos;

        this.anglesAtMoveStart = this.transform.localEulerAngles;
        this.positionAtMoveStart = this.transform.localPosition;
        this.x = Mathf.RoundToInt(pos.x);
        this.z = Mathf.RoundToInt(pos.z);

        if (SoundSourceManager.instance.TryGetActiveSource(out SoundSource source))
        {
            sourcePos = source.transform.localPosition;

            ExtendedKalmanFilter.State state = new ExtendedKalmanFilter.State(
                pos.x,
                pos.z,
                transform.localEulerAngles.y,
                sourcePos.x,
                sourcePos.z,
                source.transform.localEulerAngles.y,
                Constants.Kalman.V_SOURCE_START,
                Constants.Kalman.W_SOURCE_START
                );
            this.ekf.Initialize(state);
            StartCoroutine(DelayedPathPlanning(0.1f));
        }
        else
        {
            Debug.LogError("No sound source assigned as active");
        }

        PathPlanner.instance.OnPathFound += GetPath;
        PathPlanner.instance.OnPathNotFound += TryPathPlanningAgain;
    }

    // Update is called once per frame
    void Update()
    {
        if (stepTimer.Tick(Time.deltaTime, out float stepDeltaTime))
        {
            Step(stepDeltaTime);
            ExtendedKalmanFilter.State state = ekf.GetState();
            estimate.UpdatePosition(state);
            /*
            Debug.Log(string.Format("{0}, {1}",
                state.GetSourcePos().x,
                state.GetSourcePos().y));
            */
        }
        
        if (currentPath != null)
        {
            if (moveTimer.Tick(Time.deltaTime, out float moveDeltaTime))
            {
                if (TryMove())
                {
                    timeAtMoveStart = Time.time;
                    positionAtMoveStart = this.transform.localPosition;
                    anglesAtMoveStart = this.transform.localEulerAngles;
                }
            }
        }
        
        float motionInterpolation = (Time.time - timeAtMoveStart) / (moveTimer.GetMaxTime());
        //Debug.Log(interpolationRatio);
        this.transform.localPosition = Vector3.Lerp(
            positionAtMoveStart,
            new Vector3(this.x, transform.localPosition.y, this.z),
            motionInterpolation);
        
        float angleInterpolation = (Time.time - timeAtMoveStart) / (moveTimer.GetMaxTime() / 2);
        float angle = Helper.ToDegrees(Mathf.Atan2(
            this.x - positionAtMoveStart.x,
            this.z - positionAtMoveStart.z));

        if (angle > 180)
        {
            angle -= 360;
        }
        if (anglesAtMoveStart.y > 180)
        {
            anglesAtMoveStart = new Vector3(
                anglesAtMoveStart.x,
                anglesAtMoveStart.y - 360,
                anglesAtMoveStart.z);
        }

        Debug.Log(anglesAtMoveStart.y + " -> " + angle);
        this.transform.localEulerAngles = Vector3.Lerp(
            anglesAtMoveStart,
            new Vector3(0, angle, 0),
            angleInterpolation);
    }
    
    public void ResetPos(int x, int z)
    {
        this.x = x;
        this.z = z;
        this.transform.localPosition = new Vector3(
            x,
            this.transform.localPosition.y,
            z);
    }

    private bool TryMove()
    {
        if (currentPath.Count > 0)
        {
            Point p = currentPath.Peek();
            if (grid.IsFree(p.x, p.z))
            {
                this.x = p.x;
                this.z = p.z;
                currentPath.Dequeue();
                //Debug.LogWarning(string.Format("{0}, {1}", this.x, this.z));
                return true;
            }
        }
        return false;
    }

    //private int GetX() { return Mathf.RoundToInt(transform.localPosition.x); }
    //private int GetZ() { return Mathf.RoundToInt(transform.localPosition.z); }

    private IEnumerator DelayedPathPlanning(float time)
    {
        yield return new WaitForSeconds(time);
        StartPathPlanning(ekf.GetState());
        yield return null;
    }

    private void TryPathPlanningAgain(List<Point> path)
    {
        StartCoroutine(DelayedPathPlanning(0.5f));
    }

    private void GetPath(List<Point> path)
    {
        //Debug.LogWarning(string.Join(", ", path));

        currentPath = new Queue<Point>(path);
        currentPath.Dequeue(); // Remove starting position
        PathPlanner.instance.DrawPath(path);
        StartCoroutine(DelayedPathPlanning(timeBeforePlanning));
        //StartPathPlanning(ekf.GetState());
    }
    
    private void StartPathPlanning(ExtendedKalmanFilter.State state)
    {
        TerrainGrid grid = TerrainContainer.instance.GetGrid();
        var point = state.GetSourcePos();

        StartCoroutine(PathPlanner.instance.GetPath(
            grid,
            new Point(x, z),
            grid.GetCloseFree(point.x, point.y)));
    }

    private void Step(float deltaTime)
    {
        Vector3 robotPos = transform.localPosition;

        //transform.localPosition = new Vector3(robotPos.x, robotPos.y, robotPos.z - 0.5f);

        double x_r = robotPos.x;
        double y_r = robotPos.z;
        double th_r = transform.localEulerAngles.y;

        ExtendedKalmanFilter.Observation observation = ObserveSound();
        /*
        Debug.Log(string.Format("Observation: {0}, {1}",
            observation.GetDistance(),
            observation.GetAngle() * 180 / Math.PI));
        */
        ekf.Step(observation, x_r, y_r, th_r, deltaTime);
    }

    private ExtendedKalmanFilter.Observation ObserveSound()
    {
        if (SoundSourceManager.instance.TryGetActiveSource(out SoundSource source))
        {
            Vector3 sourcePos = source.transform.localPosition;
            double x_s = sourcePos.x;
            double y_s = sourcePos.z;

            Vector3 robotPos = transform.localPosition;
            double x_r = robotPos.x;
            double y_r = robotPos.z;

            double xDiff = x_s - x_r;
            double yDiff = y_s - y_r;

            //Debug.Log(xDiff);
            //Debug.Log(yDiff);

            double realDist = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
            double realAngle = Math.Atan2(yDiff, xDiff);

            //Debug.Log("Dist: " + realDist);
            //Debug.Log("Angle: " + realAngle);

            return new ExtendedKalmanFilter.Observation(
                Gaussian.Sample(realDist, Constants.Kalman.DISTANCE_ERROR),
                Gaussian.Sample(realAngle, Constants.Kalman.ANGLE_ERROR));
        }
        else
        {
            throw new System.Exception("No active sound source");
        }
    }
}
