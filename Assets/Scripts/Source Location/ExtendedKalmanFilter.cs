using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathExtended.Matrices;
using System;

public class ExtendedKalmanFilter : MonoBehaviour
{
    private const int m = 8;
    private const int n = 2;

    // f : m x 1 -> m x 1
    // processJacobian === F : m x m
    private Matrix processCovariance; // Q : m x m

    // h : m x 1 -> n x 1
    // observationJacobian === H : n x m
    // observationJacobianTransposed === H.T cached
    private Matrix observationCovariance; // R : n x n

    private Matrix predictionError; // P : m x m

    private State state; // x : m x 1
    private Observation observation; // z : n x 1

    private List<State> pastStates;
    private List<Observation> pastObservations;
    private List<Matrix> pastPredictionErrors;

    private EstimateVisual visual;

    private void Awake()
    {
        pastStates = new List<State>();
        pastObservations = new List<Observation>();
        pastPredictionErrors = new List<Matrix>();
        visual = GetComponent<EstimateVisual>();
    }

    public void Initialize(State state)
    {
        this.predictionError = InitializePredictionError();
        this.state = state;
        this.processCovariance = SetupProcessCovariance();
        this.observationCovariance = SetupObservationCovariance();
    }

    public void Step(Observation observation, double robotX, double robotY, double robotTheta, float deltaTime)
    {
        this.observation = observation;

        pastStates.Add(state);
        pastObservations.Add(observation);
        pastPredictionErrors.Add(predictionError);

        // Preliminary calculations
        Matrix processJacobian = CalculateProcessJacobian(deltaTime);

        //Debug.Log("State before prediction : " + state.ToString());
        //Debug.Log("Prediction error before prediction :\n" + predictionError.ToString());

        // Prediction step
        this.state = PredictState(deltaTime); // x
        this.predictionError = PredictError(processJacobian); // P

        //Debug.Log("State after prediction : " + state.ToString());
        //Debug.Log("Prediction error after prediction :\n" + predictionError.ToString());

        SetRobotPos(robotX, robotY, robotTheta);

        // Intermediate
        Matrix observationJacobian = CalculateObservationJacobain();
        Matrix observationJacobianTransposed = TransposeMatrix(
            observationJacobian);

        //Debug.Log("Observation jacobian : " + observationJacobian.ToString());

        // Update step
        Matrix residual = CalculateResidual(); // y
        //Debug.Log("Residual :\n" + residual.ToString());
        Matrix residualCovariance = CalculateResidualCovariance(
            observationJacobian,
            observationJacobianTransposed); // S
        //Debug.Log("Residual covariance :\n" + residualCovariance.ToString());
        Matrix gain = CalculateGain(
            observationJacobianTransposed,
            residualCovariance); // K

        //Debug.Log("Gain :\n" + gain.ToString());

        this.state = UpdateState(
            gain,
            residual);
        this.predictionError = UpdatePredictionError(
            observationJacobian,
            gain);

        visual.UpdatePosition(state);

        //Debug.Log("State after update : " + state.ToString());
        //Debug.Log("Prediction error after update :\n" + predictionError.ToString());
    }

    public float GetDist(SoundSource source)
    {
        Vector2 guessPos = state.GetSourcePos();
        Vector3 truePos = source.transform.localPosition;
        float dx = truePos.x - guessPos.x;
        float dz = truePos.z - guessPos.y; // Vector2 has x, y
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    private void SetRobotPos(double robotX, double robotY, double robotTheta)
    {
        Matrix mat = state.AsMatrix();
        mat[1, 1] = robotX;
        mat[2, 1] = robotY;
        mat[3, 1] = robotTheta;
        this.state = new State(mat);
    }

    #region Getters
    public State GetState() { return state; }
    public Observation GetCurrentObservation() { return observation; }
    public Matrix GetPredictionError() { return predictionError; }

    public List<State> GetStateHistory() { return pastStates; }
    public List<Observation> GetObservationHistory() { return pastObservations; }
    public List<Matrix> GetPredictionErrorHistory() { return pastPredictionErrors; }
    #endregion
    
    #region Initialization helpers
    /// P
    private Matrix InitializePredictionError()
    {
        return DiagonalMatrix(FillVector<double>(m, 1000));
    }

    /// Q
    private Matrix SetupProcessCovariance()
    {
        return DiagonalMatrix(Constants.Kalman.PROCESS_COVARIANCE_DEFAULT);
    }

    /// R
    private Matrix SetupObservationCovariance()
    {
        return DiagonalMatrix(Constants.Kalman.OBSERVATION_COVARIANCE_DEFAULT);
    }
    #endregion

    #region Predict
    private State PredictState(float deltaTime)
    {
        Vector2 robotPos = state.GetRobotPos();
        double th_r = state.GetRobotOrientation();
        Vector2 sourcePos = state.GetSourcePos();
        double th_s = state.GetSourceOrientation();
        double v_s = state.GetSourceLinearVelocity();
        double w_s = state.GetSourceAngularVelocity();

        double x_s = sourcePos.x + deltaTime * Math.Cos(th_s) * v_s;
        double y_s = sourcePos.y + deltaTime * Math.Sin(th_s) * v_s;
        th_s = th_s + deltaTime * w_s;
        return new State(robotPos.x, robotPos.y, th_r,
                         x_s, y_s, th_s,
                         v_s, w_s);
    }
    
    /// P = F * P * F.T + Q
    /// m x m * m x m * m x m + m x m = m x m
    private Matrix PredictError(Matrix processJacobian)
    {
        Matrix processJacobianTransposed = processJacobian.Duplicate();
        processJacobianTransposed.Transpose();
        return processJacobian * predictionError * processJacobianTransposed + processCovariance;
    }

    #endregion

    #region Calculate update variables
    /// F
    private Matrix CalculateProcessJacobian(float deltaTime)
    {
        Matrix mat = new Matrix(m);
        mat[4, 4] = 1; // d x_s / d x_s
        mat[5, 5] = 1; // d y_s / d y_s
        mat[6, 6] = 1; // d th_s / d th_s

        double th_s = state.GetSourceOrientation();
        double v_s = state.GetSourceLinearVelocity();

        mat[4, 6] = -deltaTime * Math.Sin(th_s) * v_s;
        mat[4, 7] = deltaTime * Math.Cos(th_s);

        mat[5, 6] = deltaTime * Math.Cos(th_s) * v_s;
        mat[5, 7] = deltaTime * Math.Sin(th_s);

        mat[6, 8] = deltaTime;
        return mat;
    }
    
    /// H
    private Matrix CalculateObservationJacobain()
    {
        Matrix mat = new Matrix(n, m);
        Vector2 robotPos = state.GetRobotPos();
        Vector2 sourcePos = state.GetSourcePos();
        double x_r = robotPos.x;
        double y_r = robotPos.y;
        double x_s = sourcePos.x;
        double y_s = sourcePos.y;

        double dx = x_s - x_r;
        double dy = y_s - y_r;
        double dx_sqr = dx * dx;
        double dy_sqr = dy * dy;
        
        double alpha = Math.Sqrt(dx_sqr + dy_sqr);
        double beta = dx * ((dy_sqr / dx_sqr) + 1);

        mat[1, 1] = -dx / alpha;
        mat[1, 2] = -dy / alpha;
        mat[1, 4] = dx / alpha;
        mat[1, 5] = dy / alpha;
        mat[2, 1] = dy / (dx * beta);
        mat[2, 2] = -1 / beta;
        mat[2, 4] = -dy / (dx * beta);
        mat[2, 5] = 1 / beta;
        return mat;
    }

    /// y = z - h(x)
    /// n x 1 - n x 1 = n x 1
    private Matrix CalculateResidual()
    {
        Matrix mat = new Matrix(n, 1);
        Vector2 robotPos = state.GetRobotPos();
        Vector2 sourcePos = state.GetSourcePos();
        double x_r = robotPos.x;
        double y_r = robotPos.y;
        double x_s = sourcePos.x;
        double y_s = sourcePos.y;

        double dx = x_s - x_r;
        double dy = y_s - y_r;

        mat[1, 1] = this.observation.GetDistance() - Math.Sqrt(dx * dx + dy * dy);
        mat[2, 1] = this.observation.GetAngle() - Math.Atan2(dy, dx);
        return mat;
    }

    /// S = H * P * H.T + R
    /// n x m * m x m * m x n + n x n = n x n
    private Matrix CalculateResidualCovariance(Matrix observationJacobian, Matrix observationJacobianTransposed)
    {
        return observationJacobian * predictionError * observationJacobianTransposed + observationCovariance;
    }

    /// K = P * H.T * S^-1
    /// m x m * m x n * n x n = m x n
    private Matrix CalculateGain(Matrix observationJacobianTransposed, Matrix residualCovariance)
    {
        residualCovariance.Inverse();

        //Debug.LogError("Prediction error: " + MatrixDims(predictionError));
        //Debug.LogError("Observation jacobian: " + MatrixDims(observationJacobianTransposed));
        //Debug.LogError("Residual covariance: " + MatrixDims(residualCovariance));

        return predictionError * observationJacobianTransposed * residualCovariance;
    }

    #endregion
    
    #region Update
    // x = x + Ky
    // m x n * n x 1 = m x 1
    private State UpdateState(Matrix gain, Matrix residual)
    {
        Matrix result = state.AsMatrix() + gain * residual;
        return new State(result);
    }

    // P = (I - KH)P
    // (m x m - m x n * n x m) * m x m = m x m
    private Matrix UpdatePredictionError(Matrix observationJacobian, Matrix gain)
    {
        return (Matrix.Identity(m) - gain * observationJacobian) * predictionError;
    }

    #endregion

    #region Structs
    public readonly struct Observation
    {
        private readonly double[] vals;

        public Observation(double rSource, double phiSource)
        {
            vals = new double[2];
            vals[0] = rSource;
            vals[1] = phiSource;
        }

        public Observation(double[] vals)
        {
            this.vals = vals;
        }
        
        public Observation(Matrix m)
        {
            vals = new double[m.Rows];
            for (int i = 0; i < m.Rows; i++)
            {
                vals[i] = m[i + 1, 1];
            }
        }

        public Matrix AsMatrix()
        {
            Matrix m = new Matrix(vals.Length, 1);
            for (int i = 0; i < vals.Length; i++)
            {
                m[i + 1, 1] = vals[i];
            }
            return m;
        }

        public double GetDistance()
        {
            return vals[0];
        }

        // Radians
        public double GetAngle()
        {
            return vals[1];
        }
    }

    public readonly struct State
    {
        private readonly double[] vals;

        public State(double xRobot, double yRobot, double thetaRobot,
                     double xSource, double ySource, double thetaSource,
                     double linVelSource, double angVelSource)
        {
            vals = new double[8];
            vals[0] = xRobot;
            vals[1] = yRobot;
            vals[2] = thetaRobot;
            vals[3] = xSource;
            vals[4] = ySource;
            vals[5] = thetaSource;
            vals[6] = linVelSource;
            vals[7] = angVelSource;
        }

        public State(double[] vals)
        {
            this.vals = vals;
        }

        public State(Matrix m)
        {
            vals = new double[m.Rows];
            for (int i = 0; i < m.Rows; i++)
            {
                vals[i] = m[i + 1, 1];
            }
        }

        public Matrix AsMatrix()
        {
            Matrix m = new Matrix(vals.Length, 1);
            for (int i = 0; i < vals.Length; i++)
            {
                m[i + 1, 1] = vals[i];
            }
            return m;
        }

        public double[] GetVals()
        {
            return vals;
        }

        public override string ToString()
        {
            string str = "";
            foreach (double d in vals)
            {
                str += d + ", ";
            }
            return str;
        }

        public Vector2 GetRobotPos()
        {
            return new Vector2((float)vals[0], (float)vals[1]);
        }

        /// Radians
        public double GetRobotOrientation()
        {
            return vals[2];
        }

        public Vector2 GetSourcePos()
        {
            return new Vector2((float)vals[3], (float)vals[4]);
        }

        /// Radians
        public double GetSourceOrientation()
        {
            return vals[5];
        }

        /// Unit / sec
        public double GetSourceLinearVelocity()
        {
            return vals[6];
        }

        /// Radians / sec
        public double GetSourceAngularVelocity()
        {
            return vals[7];
        }
    }

    #endregion

    #region Static methods
    private static Matrix DiagonalMatrix(double[] v)
    {
        Matrix mat = new Matrix(v.Length);
        for(int i = 0; i < v.Length; i++)
        {
            mat[i + 1, i + 1] = v[i];
        }
        return mat;
    }

    private static T[] FillVector<T>(int size, T value)
    {
        T[] arr = new T[size];
        for (int i = 0; i < size; i++)
        {
            arr[i] = value;
        }
        return arr;
    }

    private static Matrix TransposeMatrix(Matrix m)
    {
        Matrix temp = m.Duplicate();
        temp.Transpose();
        return temp;
    }

    private static string MatrixDims(Matrix m)
    {
        return string.Format("{0}, {1}", m.Rows, m.Columns);
    }
    #endregion
}
