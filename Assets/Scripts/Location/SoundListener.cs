using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundListener : MonoBehaviour
{
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private EstimateVisual estimate;

    private ExtendedKalmanFilter ekf;

    private void Awake()
    {
        this.ekf = new ExtendedKalmanFilter();
    }

    private void Start()
    {
        Vector3 pos = transform.localPosition;
        Vector3 sourcePos;
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
        }
        else
        {
            Debug.LogError("No sound source assigned as active");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.Tick(Time.deltaTime, out float time))
        {
            Step(time);
            ExtendedKalmanFilter.State state = ekf.GetState();
            estimate.UpdatePosition(state);
        }
    }

    private void Step(float deltaTime)
    {
        Vector3 robotPos = transform.localPosition;

        transform.localPosition = new Vector3(robotPos.x, robotPos.y, robotPos.z - 0.5f);

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

            //Debug.LogError(xDiff);
            //Debug.LogError(yDiff);

            double realDist = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
            double realAngle = Math.Atan2(yDiff, xDiff);

            //Debug.LogError("Dist: " + realDist);
            //Debug.LogError("Angle: " + realAngle);

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
