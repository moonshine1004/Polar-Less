using UnityEngine;

public class BootStrap : MonoBehaviour
{
    [SerializeField] private LightView _lightView;
    [SerializeField] private MirrorView _mirrorPrefab;
    [SerializeField] private ObjectSpriteCatalogSO _mirrorSpriteCatalog;
    
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

            // mirrorUseCase
            var mirrorFactory = new ObjectFactory(mirrorPool, _mirrorSpriteCatalog);
            // infraUseCase

        #endregion

        #region Services
            // lightServices
            lightServices = new LightServices();

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