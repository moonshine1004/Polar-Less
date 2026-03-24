using System;
using System.Collections.Generic;
using UnityEngine;

public class LightServices
{
    private readonly Dictionary<Type, object> _services = new();

    public void Register<T>(T sevice) where T : class
    {
        _services[typeof(T)] = sevice;
    }
    public T Get<T>() where T : class => (T)_services[typeof(T)];
}

public class LightPathTracerUseCase
{
    
}




