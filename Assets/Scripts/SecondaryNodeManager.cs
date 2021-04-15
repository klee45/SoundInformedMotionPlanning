using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryNodeManager : Singleton<SecondaryNodeManager>
{
    private SecondarySource[] sources;
    private Dictionary<SecondarySource, List<SecondarySource>> edges;

    protected override void Awake()
    {
        base.Awake();
        sources = GetComponentsInChildren<SecondarySource>();
        edges = new Dictionary<SecondarySource, List<SecondarySource>>();
    }

    private void Start()
    {
        
    }
}
