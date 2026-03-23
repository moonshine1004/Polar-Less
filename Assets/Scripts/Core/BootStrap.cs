using UnityEngine;

public class BootStrap : MonoBehaviour
{
    [SerializeField] private LightView _lightView;
    [SerializeField] private MirrorView _mirrorPrefab;
    [SerializeField] private MirrorSpriteCatalogSO _mirrorSpriteCatalog;
    
    public LightServices lightServices { get; private set; }

    private void Awake()
    {
        #region Infra
            var mirrorPool = new MirrorPooling(_mirrorPrefab);
        #endregion
        
        #region Domains
        var lightDomain = new LightDomain();
        #endregion

        #region UseCases
            // lightUseCase
            var lightStartUseCase = new LightStartUseCase();
            var lightReflectUseCase = new LightReflectUseCase();
            // mirrorUseCase
            var mirrorFactory = new MirrorFactory(mirrorPool, _mirrorSpriteCatalog);
            // infraUseCase

        #endregion

        #region Services
            // lightServices
            lightServices = new LightServices();
            lightServices.Register(lightStartUseCase);
            lightServices.Register(lightReflectUseCase);
            // mirrorServices


        #endregion

        #region Views
            // lightView
            _lightView.InstallLightView(lightDomain, lightServices);
            // mittotrView
            var mirrorEventBus = new EventBus();

        #endregion

        #region TestCase
            var mirrorTestCase = FindObjectOfType<MirrorTestCase>();
            mirrorTestCase.Install(mirrorFactory);
        #endregion

    }
}