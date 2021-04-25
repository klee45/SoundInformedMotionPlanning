using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryEchoSourceManager : Singleton<SecondaryEchoSourceManager>
{
    private SecondaryEchoSource[] sources;
    private Dictionary<SecondaryEchoSource, List<SecondaryEchoSource>> edges;

    protected override void Awake()
    {
        base.Awake();
        sources = GetComponentsInChildren<SecondaryEchoSource>();
        edges = new Dictionary<SecondaryEchoSource, List<SecondaryEchoSource>>();
    }

    private void Start()
    {
        
    }
}
