using UnityEngine;

/// <summary>
/// 스테이지 매니저가 참조하여 MirrorView를 생성하는 역할
/// </summary>
public class ObjectFactory
{
    public ObjectFactory(IObjectPool<MirrorView> mirrorPooling, ObjectSpriteCatalogSO objectSpriteCatalog, IRegistMirror mirrorRegistry)
    {
        _mirrorPool = mirrorPooling;
        _objectSpriteCatalog = objectSpriteCatalog;
        _mirrorRegistry = mirrorRegistry;
    }
    
    private readonly IMirrorMoveStrategy _rotateMoveStrategy = new RotateMirrorMoveStrategy();
    private readonly IMirrorMoveStrategy _slideMoveStrategy = new SlideMirrorMoveStrategy();
    private readonly IObjectPool<MirrorView> _mirrorPool;
    private readonly IRegistMirror _mirrorRegistry;
    private readonly ObjectSpriteCatalogSO _objectSpriteCatalog;
    
    /// <summary>
    /// MirrorView를 풀에서 가져와서, MirrorDomain과 MoveStrategy를 주입하여 초기화한 후 반환하는 역할
    /// </summary>
    public void CreateObject(ObjectData mirrorData)
    {
        switch (mirrorData.objectType)
        {
            case ObjectType.Rotate:
                CreateRotateMirror(mirrorData);
                break;
            case ObjectType.Slide:
                CreateSlideMirror(mirrorData);
                break;
            case ObjectType.Obstacle:
                // 장애물 생성 로직 추가 예정
                break;
            case ObjectType.Goal:
                // Goal 생성 로직 추가 예정
                break;
            default:
                return;
        }
    }

    private void CreateRotateMirror(ObjectData mirrorData)
    {
        // MirrorView를 풀에서 가져옴(껍데기만)
        MirrorView mirrorView = _mirrorPool.Get();
        // 데이터 생성
        MirrorDomain mirrorDomain = new MirrorDomain();
        mirrorDomain.Init(mirrorData.ID, mirrorData.position, mirrorData.rotation);

        // MirrorView에 데이터와 전략 주입
        mirrorView.Install(mirrorDomain, _rotateMoveStrategy, _objectSpriteCatalog.RotateMirrorSprite);

        _mirrorRegistry.RegisterMirror(mirrorView);
    }
    public void CreateSlideMirror(ObjectData mirrorData)
    {
        MirrorView mirrorView = _mirrorPool.Get();

        MirrorDomain mirrorDomain = new MirrorDomain();
        mirrorDomain.Init(mirrorData.ID, mirrorData.position, mirrorData.rotation);

        mirrorView.Install(mirrorDomain, _slideMoveStrategy, _objectSpriteCatalog.SlideMirrorSprite);

        _mirrorRegistry.RegisterMirror(mirrorView);
    }
}