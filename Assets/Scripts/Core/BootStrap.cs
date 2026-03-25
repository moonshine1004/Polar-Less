using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BootStrap : MonoBehaviour
{
    [Header("유니티 에셋 주입")]
    [SerializeField] private GameStageCatalogSO _gameStageCatalog;
    [SerializeField] private MirrorView _mirrorPrefab;
    [SerializeField] private ObjectSpriteCatalogSO _mirrorSpriteCatalog;
    [Header("게임 오브젝트")]
    [SerializeField] private LightView _lightView;
    [SerializeField] private TestCase _testCase;
    [SerializeField] private GoalView _goalView;
    
    public LightServices lightServices { get; private set; }

    private void Awake()
    {
        #region Infra
            var mirrorPool = new ObjectPooling(_mirrorPrefab);
        #endregion
        
        #region Domains
            var gameStageObjectRegistry = new GameStageObjectRegistry();
            var lightDomain = new LightDomain();

        #endregion

        #region UseCases
            
            // lightUseCase

            // mirrorUseCase
            var objectFactory = new ObjectFactory(mirrorPool, _mirrorSpriteCatalog, gameStageObjectRegistry);
            // infraUseCase

            // ochestrator
            var gameStageServices = new GameStageServices(_gameStageCatalog, objectFactory, mirrorPool, gameStageObjectRegistry);
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
            // goalView
            _goalView.Install(gameStageServices);

        #endregion

        #region TestCase
            _testCase.Install(gameStageServices);
        #endregion

    }
}