using UnityEngine;

public class BootStrap : MonoBehaviour
{
    [SerializeField] private LightView _lightView;
    [SerializeField] private MirrorPooling _mirrorPooling;
    
    public LightServices lightServices { get; private set; }
    public MirrorServices mirrorServices { get; private set; }

    private void Awake()
    {
        // Infra
        
        
        #region Domains
        var lightDomain = new LightDomain();
        #endregion

        #region UseCases
        // lightUseCase
        var lightStartUseCase = new LightStartUseCase();
        var lightReflectUseCase = new LightReflectUseCase();
        // mirrorUseCase
        var mirrorInstallUseCase = new MirrorInstallUseCase();

        // infraUseCase

        var rotateMirrorPool = _mirrorPooling;
        #endregion

        #region Services
        // lightServices
        lightServices = new LightServices();
        lightServices.Register(lightStartUseCase);
        lightServices.Register(lightReflectUseCase);
        // mirrorServices
        mirrorServices = new MirrorServices();
        mirrorServices.Register(mirrorInstallUseCase);

        #endregion

        #region Views
        // lightView
        _lightView.InstallLightView(lightDomain, lightServices);
        // mittotrView
        var mirrorEventBus = new EventBus();
        _mirrorPooling.Install(mirrorEventBus);
        #endregion

    }
}