using UnityEngine;
using GamePlay.Light.Domain;

namespace GamePlay.Light.UseCase
{
    
    public class LightReflectUseCase
    {
        public LightDrawData Excute(LightView lightView, LightDomain lightDomain, Vector2 hitPoint, Vector2 hitNormal)
        {
            lightDomain.PathIndex++;
            lightView.LineRenderer.positionCount = lightDomain.PathIndex;
            

            return new LightDrawData
            {
                Index = lightDomain.PathIndex - 2,
                Origin = hitPoint,
                Direction = Vector2.Reflect(hitPoint, hitNormal),
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

}