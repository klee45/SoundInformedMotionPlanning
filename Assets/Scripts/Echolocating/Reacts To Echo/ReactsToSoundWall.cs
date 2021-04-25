using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactsToSoundWall : ReactsToEcho
{
    private Wall wall;

    public void Setup(Wall wall)
    {
        this.wall = wall;
    }

    public override void React(EchoSource source)
    {
        wall.React(source);
    }
}
