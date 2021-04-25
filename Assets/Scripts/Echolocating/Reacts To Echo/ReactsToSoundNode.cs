using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactsToEchoNode : ReactsToEcho
{
    private SecondaryEchoSource node;

    public void Setup(SecondaryEchoSource node)
    {
        this.node = node;
    }

    public override void React(EchoSource source)
    {
        node.React(source);
    }
}
