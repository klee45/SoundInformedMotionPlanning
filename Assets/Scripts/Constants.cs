using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    // Extended Kalman Fitler ---------------------------------------
    public static class Kalman
    {
        public static readonly double[] PROCESS_COVARIANCE_DEFAULT = new List<double>
        {// x_r   y_r   th_r  x_s   y_s   th_s  v_s   w_s
            1e-2, 1e-2, 5e-1, 6e-2, 6e-2, 9e-1, 1e-2, 5e-1
        }.ToArray();

        public static readonly double[] OBSERVATION_COVARIANCE_DEFAULT = new List<double>
        {// r_s   phi_s
            1e-0, 1e-0
        }.ToArray();
    }

    // Layers -------------------------------------------------------

    public static class Layers
    {
        public const int SOURCE_ON = 8;
        public const int SOURCE_OFF = 9;

        public const int WALL_ON = 10;
        public const int WALL_OFF = 11;

        public const int WALL_RAYCAST = 13;
    }

    // Values -------------------------------------------------------
    public static class Values
    {
        public const float LEARNING_RATE = 0.5f;

        public const float FOUND_FREE = 0.2f;
        public const float FOUND_WALL = 0.8f;

        public const float ECHOLOCATE_RATE = 10f; // 1/10th of the total sets per update
        public const int NOISY_MAP_RANGE = 2; // 5x5 range around hit

        public const float WAVE_TIME = 1.5f;
        public const float ENABLE_TIME = 2.5f;
    }

    // Colors -------------------------------------------------------

    public static class Colors
    {
        public static Color MAP_WALL = new Color(0.5f, 0, 0);
        public static Color MAP_FREE = new Color(0, 0.5f, 0);

        public static Color SOURCE_ON = Color.white;
        public static Color SOURCE_OFF = Color.gray;

        public static Color WALL_ON = Color.white;
        public static Color WALL_OFF = Color.gray;
    }
}
