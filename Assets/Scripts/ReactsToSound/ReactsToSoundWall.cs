using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactsToSoundWall : ReactsToSound
{
    private Wall wall;

    public void Setup(Wall wall)
    {
        this.wall = wall;
    }

    public override void React(SoundSource source)
    {
        wall.React(source);
    }
}
