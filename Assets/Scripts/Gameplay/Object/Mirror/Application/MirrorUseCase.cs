using System;
using System.Collections.Generic;
using UnityEngine;

public class MirrorServices
{
    private readonly Dictionary<Type, object> _services = new();

    public void Register<T>(T sevice) where T : class
    {
        _services[typeof(T)] = sevice;
    }
    public T Get<T>() where T : class => (T)_services[typeof(T)];
}

public class MirrorInstallUseCase // 풀에서 꺼낼 때
{
    public MirrorDomain Excute()
    {
        var mirrorDomain = new MirrorDomain();
        mirrorDomain.MirrorType = MirrorType.Rotate;
        mirrorDomain.MirrorID = 0;
        return mirrorDomain;
    }
}
