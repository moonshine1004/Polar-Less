using UnityEngine;
using GamePlay.Light.UseCase;
using GamePlay.Light.Domain;

public class BootStrap : MonoBehaviour
{
    [SerializeField] private LightView _lightView;
    private void Awake()
    {
        // Domains
        var lightDomain = new LightDomain();

        // UseCases
        var lightStartUseCase = new LightStartUseCase();

        // Views
        _lightView.InstallLightView(lightDomain, lightStartUseCase);
    }
}