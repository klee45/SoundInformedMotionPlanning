﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    // Debug --------------------------------------------------------
    public static class Debug
    {
        public static readonly bool SHOW_ECHOLOCATION_MESSAGES = false;
    }

    // Extended Kalman Fitler ---------------------------------------
    public static class Kalman
    {
        public const double X_SOURCE_START = 1;
        public const double Y_SOURCE_START = 2;
        public static readonly double THETA_SOURCE_START = 0;
        public const double V_SOURCE_START = 0.5;
        public const double W_SOURCE_START = 0;

        public const double X_ROBOT_ERROR = 1e-1;
        public const double Y_ROBOT_ERROR = 1e-1;
        public static readonly double THETA_ROBOT_ERROR = Helper.ToRadians(15);

        public const double X_SOURCE_ERROR = 5e-1;
        public const double Y_SOURCE_ERROR = 5e-1;
        public static readonly double THETA_SOURCE_ERROR = Helper.ToRadians(30);

        public const double V_SOURCE_ERROR = 1e-1;
        public static readonly double W_SOURCE_ERROR = Helper.ToRadians(15);

        public static readonly double[] PROCESS_COVARIANCE_DEFAULT = new List<double>
        {
            X_ROBOT_ERROR * X_ROBOT_ERROR, // x_r
            Y_ROBOT_ERROR * Y_ROBOT_ERROR, // y_r
            THETA_ROBOT_ERROR * THETA_ROBOT_ERROR, // th_r

            X_SOURCE_ERROR * X_SOURCE_ERROR, // x_s
            Y_SOURCE_ERROR * Y_SOURCE_ERROR, // y_s
            THETA_SOURCE_ERROR * THETA_SOURCE_ERROR, // th_s

            V_SOURCE_ERROR * V_SOURCE_ERROR, // v_s
            W_SOURCE_ERROR * W_SOURCE_ERROR, // w_s
        }.ToArray();

        public const double DISTANCE_ERROR = 1e-0;
        public static double ANGLE_ERROR = Math.Sqrt(15 * Math.PI / 180f);

        public static readonly double[] OBSERVATION_COVARIANCE_DEFAULT = new List<double>
        {
            DISTANCE_ERROR * DISTANCE_ERROR, // r_s
            ANGLE_ERROR * ANGLE_ERROR, // phi_s
        }.ToArray();
    }

    // Layers -------------------------------------------------------

    public static class Layers
    {
        public const int SOURCE_ON = 8;
        public const int SOURCE_OFF = 9;

        public const int TERRAIN_ON = 10;
        public const int TERRAIN_OFF = 11;

        public const int TERRAIN_RAYCAST = 13;
    }

    // Values -------------------------------------------------------
    public static class Values
    {
        public const float LEARNING_RATE = 20f;

        public const float FOUND_FREE = 0.2f;
        public const float FOUND_WALL = 0.8f;

        public const float ECHOLOCATE_RATE = 10f; // 1/10th of the total sets per update
        public const int NOISY_MAP_RANGE = 0; // 5x5 range around hit

        public const float WAVE_TIME = 1.5f;
        public const float ENABLE_TIME = 2.5f;
    }

    // Colors -------------------------------------------------------

    public static class Colors
    {
        public static Color MAP_WALL = new Color(0.5f, 0, 0);
        public static Color MAP_FREE = new Color(0, 0.5f, 0);

        private const float SOURCE_OPACITY = 0.2f;
        public static Color SOURCE_ON = new Color(1, 1, 1, SOURCE_OPACITY);
        public static Color SOURCE_OFF = new Color(0.5f, 0.5f, 0.5f, SOURCE_OPACITY);

        private const float TERRAIN_OPACITY = 0.95f;
        private const float TERRAIN_ON = 0.2f;
        private const float TERRAIN_OFF = 0.1f;
        private const float TERRAIN_TOTAL_OFF = 0.05f;

        public static Color WALL_ON = new Color(TERRAIN_ON, TERRAIN_ON / 2, TERRAIN_ON / 2, TERRAIN_OPACITY);
        public static Color WALL_OFF = new Color(TERRAIN_OFF, 0, 0, TERRAIN_OPACITY);
        public static Color WALL_TOTAL_OFF = new Color(TERRAIN_TOTAL_OFF, 0, 0, TERRAIN_OPACITY);

        public static Color FLOOR_ON = new Color(TERRAIN_ON / 2, TERRAIN_ON, TERRAIN_ON / 2, TERRAIN_OPACITY);
        public static Color FLOOR_OFF = new Color(0, TERRAIN_OFF, 0, TERRAIN_OPACITY);
        public static Color FLOOR_TOTAL_OFF = new Color(0, TERRAIN_TOTAL_OFF, 0, TERRAIN_OPACITY);
    }
}
