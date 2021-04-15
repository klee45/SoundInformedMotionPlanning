using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactsToSoundNode : ReactsToSound
{
    private SecondarySource node;

    public void Setup(SecondarySource node)
    {
        this.node = node;
    }

    public override void React(SoundSource source)
    {
        node.React(source);
    }
}
