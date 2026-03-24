using UnityEngine;

/// <summary>
/// 스테이지 매니저가 참조하여 MirrorView를 생성하는 역할
/// </summary>
public class ObjectFactory
{
    public ObjectFactory(IMirrorPool<MirrorView> mirrorPooling, ObjectSpriteCatalogSO mirrorSpriteCatalog)
    {
        _mirrorPool = mirrorPooling;
        _mirrorSpriteCatalog = mirrorSpriteCatalog;
    }
    
    private readonly IMirrorMoveStrategy _rotateMoveStrategy = new RotateMirrorMoveStrategy();
    private readonly IMirrorMoveStrategy _slideMoveStrategy = new SlideMirrorMoveStrategy();
    private readonly IMirrorPool<MirrorView> _mirrorPool;
    private readonly ObjectSpriteCatalogSO _mirrorSpriteCatalog;
    
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
        mirrorDomain.Init(mirrorData.ID, mirrorData.Position, mirrorData.Rotation);

        // MirrorView에 데이터와 전략 주입
        mirrorView.Install(mirrorDomain, _rotateMoveStrategy, _mirrorSpriteCatalog.RotateMirrorSprite);
    }
    public void CreateSlideMirror(ObjectData mirrorData)
    {
        MirrorView mirrorView = _mirrorPool.Get();

        MirrorDomain mirrorDomain = new MirrorDomain();
        mirrorDomain.Init(mirrorData.ID, mirrorData.Position, mirrorData.Rotation);

        mirrorView.Install(mirrorDomain, _slideMoveStrategy, _mirrorSpriteCatalog.SlideMirrorSprite);
    }
}