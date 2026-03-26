using System.Collections.Generic;
using UnityEngine;

public class LightDomain
{
    private List<Vector3> _lightPath = new List<Vector3>(); 
    private float _maxDistance = 30f; 
    private int _maxReflections = 18;

    public List<Vector3> LightPath
    {
        get => _lightPath;
        set => _lightPath = value;
    }

    public float MaxDistance => _maxDistance;
    public int MaxReflections => _maxReflections;

}
