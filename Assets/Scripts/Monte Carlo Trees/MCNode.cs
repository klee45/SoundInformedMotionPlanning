using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCNode : MonoBehaviour
{
    private MCPose robotPose;
    private Point sourcePos;
    private List<MCAction> actions;
    private float reward;
    private int visitCount;

    private List<MCPose> children;

    public MCNode(int x, int y)
    {

    }

    public readonly struct MCPose
    {
        public readonly Point position;
        public readonly Vector2 direction;
        public MCPose(Point position, Vector2 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    public class MCAction
    {

    }
}
