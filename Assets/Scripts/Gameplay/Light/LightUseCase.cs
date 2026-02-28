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

public class LightReflectUseCase
{
    /// <summary>
    /// 레이저가 거울에 닿았을 때, 반사되는 레이저의 경로를 계산하여 반환하는 유스케이스
    /// </summary>
    /// <param name="lightView">positionCount를 받아옴</param>
    /// <param name="lightDomain">기본 정책</param>
    /// <param name="hitPoint">충돌 지점</param>
    /// <param name="hitDirection">입사각</param>
    /// <param name="hitNormal">법선</param>
    /// <returns></returns>
    public LightDrawData Excute(LightView lightView, LightDomain lightDomain,Vector2 hitPoint ,Vector2 hitDirection, Vector2 hitNormal)
    {
        lightDomain.PathIndex++;
        lightView.LineRenderer.positionCount = lightDomain.PathIndex;
        
        return new LightDrawData
        {
            Index = lightDomain.PathIndex - 2,
            Origin =  hitPoint + hitNormal * 0.001f,
            Direction = Vector2.Reflect(hitDirection, hitNormal),
            MaxDistance = lightDomain.MaxDistance
        };
    }
}

public class LightStartUseCase
{
    public LightDrawData Excute(LightView lightView, LightDomain lightDomain)
    {
        lightDomain.PathIndex = 2;
        lightView.LineRenderer.positionCount = lightDomain.PathIndex;

        lightDomain.LightPath.Add(lightView.transform.position);
        lightDomain.LightPath.Add(lightView.transform.position + Vector3.up * lightDomain.MaxDistance);

        return new LightDrawData
        {
            Index = 0,
            Origin = lightView.transform.position,
            Direction = Vector2.up,
            MaxDistance = lightDomain.MaxDistance
        };
    }
}


