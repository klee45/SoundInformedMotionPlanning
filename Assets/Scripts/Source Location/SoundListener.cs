using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is an overloaded class for moving the robot
/// towards the sound source (the player)
/// 
/// The robot alternates between taking the A* path (pink)
/// and the Monte Carlo path (yellow). I believe that it would
/// converge eventually with just the Monte Carlo path, but
/// often it gets stuck in local minima and so it needs some
/// outside push to get it out of those spots.
/// 
/// This class also controls movement animation and interpolation
/// </summary>
public class SoundListener : MonoBehaviour
{
    [SerializeField]
    private Timer stepTimer;
    [SerializeField]
    private Timer moveTimer;
    [SerializeField]
    private float timeBeforePlanning;

    [SerializeField]
    private float minTimeMod = 0.7f;
    [SerializeField]
    private float timeDecreasePerSec = 0.95f;

    /*
    [SerializeField]
    private float startStepTime = 5f;
    [SerializeField]
    private float minStepTime = 0.1f;
    [SerializeField]
    private float stepTimeDecrease = 0.5f;
    */

    private Animator animator;

    [SerializeField]
    private NoisyGrid grid;

    [SerializeField]
    private int aStarSteps = 4;
    [SerializeField]
    private int monteCarloSteps = 4;

    private int stepsDuringMode;

    private Queue<Point> currentAStarPath;
    private Queue<Point> currentMonteCarloPath;

    private bool isMonteCarloMode = false;

    private int x;
    private int z;

    private Vector3 anglesAtMoveStart;
    private Vector3 positionAtMoveStart;
    private float timeAtMoveStart;

    [SerializeField]
    private GameObject ekfContainer;

    private ExtendedKalmanFilter[] ekfs;
    private ExtendedKalmanFilter bestEkf;

    private bool isActive = false;
    private bool canMove = false;
    private bool isMoving = false;

    private float speed;
    private float startingTime;
    private float timeMod = 1.0f;

    private void Awake()
    {
        ekfs = ekfContainer.GetComponentsInChildren<ExtendedKalmanFilter>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Activate(bool val)
    {
        this.isActive = val;
    }

    private void Start()
    {
        speed = 0;
        startingTime = Time.time;
        stepsDuringMode = 0;

        //stepTimer.SetMaxTime(startStepTime);
        stepTimer.SetMaxTime(Constants.Timing.STEP_TIME);
        moveTimer.SetMaxTime(Constants.Timing.MOVE_TIME);
        Vector3 pos = transform.localPosition;
        this.anglesAtMoveStart = this.transform.localEulerAngles;
        this.positionAtMoveStart = this.transform.localPosition;
        this.x = Mathf.RoundToInt(pos.x);
        this.z = Mathf.RoundToInt(pos.z);

        StartPathPlanning();
        
        PathPlanner.instance.OnPathFound += GetPath;
        PathPlanner.instance.OnPathNotFound += TryPathPlanningAgain;
    }

    public void AllowMovement()
    {
        canMove = true;
    }

    public void StartPathPlanning()
    {
        if (SoundSourceManager.instance.TryGetActiveSource(out SoundSource source))
        {
            foreach (ExtendedKalmanFilter ekf in ekfs)
            {
                int sourceX = UnityEngine.Random.Range(0, grid.GetWidth());
                int sourceY = UnityEngine.Random.Range(0, grid.GetHeight());
                ekf.Initialize(GenerateInitialState(source, sourceX, sourceY), true);
            }
            bestEkf = ekfs[0];
            StartCoroutine(DelayedPathPlanning(0.1f));
        }
        else
        {
            Debug.Log("No sound source assigned as active");
        }
    }

    private ExtendedKalmanFilter.State GenerateInitialState(SoundSource source, int sourceX, int sourceY)
    {
        Vector3 pos = transform.localPosition;

        ExtendedKalmanFilter.State state = new ExtendedKalmanFilter.State(
            pos.x,
            pos.z,
            transform.localEulerAngles.y,
            sourceX,
            sourceY,
            source.transform.localEulerAngles.y,
            Constants.Kalman.V_SOURCE_START,
            Constants.Kalman.W_SOURCE_START
            );
        return state;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (stepTimer.Tick(Time.deltaTime, out float stepDeltaTime))
            {
                Step(stepDeltaTime);
                ExtendedKalmanFilter.State state = bestEkf.GetState();
            }

            if (currentAStarPath != null)
            {
                if (canMove)
                {
                    if (moveTimer.Tick(Time.deltaTime, out float moveDeltaTime))
                    {
                        if (TryMove())
                        {
                            timeAtMoveStart = Time.time;
                            positionAtMoveStart = this.transform.localPosition;
                            anglesAtMoveStart = this.transform.localEulerAngles;
                            isMoving = true;
                            stepsDuringMode++;
                            CheckModeSwitch();
                        }
                        else
                        {
                            isMoving = false;
                        }
                    }
                }
            }
            if (isMoving)
            {
                DoMovement();
            }
            else
            {
                speed = 0;
                animator.SetFloat("speed", speed);
            }
        }
    }

    private void DoMovement()
    {
        float timeDiff = Time.time - timeAtMoveStart;
        float motionInterpolation = timeDiff / (moveTimer.GetMaxTime());
        Vector3 positionTarget = new Vector3(this.x, transform.localPosition.y, this.z);
        //Debug.Log(interpolationRatio);
        this.transform.localPosition = Vector3.Lerp(
            positionAtMoveStart,
            positionTarget,
            motionInterpolation);

        // Can't just subtract the vectors because we don't want y
        float dx = positionAtMoveStart.x - positionTarget.x;
        float dz = positionAtMoveStart.z - positionTarget.z;
        float distPerSec = Mathf.Max(0, (Mathf.Sqrt(dx * dx + dz * dz) - 1)) / moveTimer.GetMaxTime();
        
        speed += 1f * Time.deltaTime;

        animator.SetFloat("speed", speed);
        //Debug.Log(speed);

        timeMod = Mathf.Max(minTimeMod, timeMod * Mathf.Pow(timeDecreasePerSec, 1 + timeDiff));
        moveTimer.SetMaxTime(Constants.Timing.MOVE_TIME * timeMod);
        stepTimer.SetMaxTime(Constants.Timing.SCAN_TIME * timeMod);

        float angleInterpolation = timeDiff / (moveTimer.GetMaxTime() * 0.75f);
        float angle = Helper.ToDegrees(Mathf.Atan2(
            this.x - positionAtMoveStart.x,
            this.z - positionAtMoveStart.z));

        if (angle < 0)
        {
            angle += 360;
        }

        if (angle - anglesAtMoveStart.y > 180)
        {
            angle -= 360;
        }

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

    public Point GetPos()
    {
        return new Point(x, z);
    }

    private bool TryMove()
    {
        Queue<Point> path;
        if (isMonteCarloMode)
        {
            path = currentMonteCarloPath;
        }
        else
        {
            path = currentAStarPath;
        }
        if (path.Count > 0)
        {
            Point p = path.Peek();
            if (grid.IsFreeTile(p.x, p.z))
            {
                this.x = p.x;
                this.z = p.z;
                path.Dequeue();
                //Debug.LogWarning(string.Format("{0}, {1}", this.x, this.z));
                return true;
            }
        }
        return false;
    }

    private void CheckModeSwitch()
    {
        if (isMonteCarloMode)
        {
            if (stepsDuringMode >= monteCarloSteps)
            {
                isMonteCarloMode = false;
                ShowAStarPath();
                stepsDuringMode = 0;
            }
        }
        else
        {
            if (stepsDuringMode >= aStarSteps)
            {
                isMonteCarloMode = true;
                MCTree.instance.ResetTree();
                MCTree.instance.Search();
                currentMonteCarloPath = MCTree.instance.GetPath();
                ShowMonteCarloPath();
                stepsDuringMode = 0;
            }
        }
    }

    private void ShowAStarPath()
    {
        PathPlanner.instance.ShowVisual();
        MCTree.instance.HideVisual();
    }

    private void ShowMonteCarloPath()
    {
        PathPlanner.instance.HideVisual();
        MCTree.instance.ShowVisual();
    }

    //private int GetX() { return Mathf.RoundToInt(transform.localPosition.x); }
    //private int GetZ() { return Mathf.RoundToInt(transform.localPosition.z); }

    private IEnumerator DelayedPathPlanning(float time)
    {
        yield return new WaitForSeconds(time);
        StartPathPlanning(bestEkf.GetState());
        yield return null;
    }

    private void TryPathPlanningAgain(List<Point> path)
    {
        StartCoroutine(DelayedPathPlanning(0.5f));
    }

    private void GetPath(List<Point> path)
    {
        //Debug.LogWarning(string.Join(", ", path));

        currentAStarPath = new Queue<Point>(path);
        if (currentAStarPath.Count > 0)
        {
            currentAStarPath.Dequeue(); // Remove starting position
        }
        if (isActive)
        {
            PathPlanner.instance.DrawPath(path, grid);
        }
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
        float bestScore = Mathf.Infinity;
        if (SoundSourceManager.instance.TryGetActiveSource(out SoundSource source))
        {
            foreach (var ekf in ekfs)
            {
                ekf.Step(observation, x_r, y_r, th_r, deltaTime);
                float score = ekf.GetDist(source);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestEkf = ekf;
                }
            }
        }
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
