using UnityEngine;

/// <summary>
/// 스테이지 매니저가 참조하여 MirrorView를 생성하는 역할
/// </summary>
public class MirrorFactory
{
    public MirrorFactory(IMirrorPool<MirrorView> mirrorPooling, MirrorSpriteCatalogSO mirrorSpriteCatalog)
    {
        _mirrorPool = mirrorPooling;
        _mirrorSpriteCatalog = mirrorSpriteCatalog;
    }
    
    private readonly IMirrorMoveStrategy _rotateMoveStrategy = new RotateMirrorMoveStrategy();
    private readonly IMirrorMoveStrategy _slideMoveStrategy = new SlideMirrorMoveStrategy();
    private readonly IMirrorPool<MirrorView> _mirrorPool;
    private readonly MirrorSpriteCatalogSO _mirrorSpriteCatalog;
    
    /// <summary>
    /// MirrorView를 풀에서 가져와서, MirrorDomain과 MoveStrategy를 주입하여 초기화한 후 반환하는 역할
    /// </summary>
    public MirrorView CreateRotateMirror(int ID, Vector3 position, Quaternion rotation)
    {
        // MirrorView를 풀에서 가져옴(껍데기만)
        MirrorView mirrorView = _mirrorPool.Get();

        // 데이터 생성
        MirrorDomain mirrorDomain = new MirrorDomain();
        mirrorDomain.Init(ID, position, rotation);

        // MirrorView에 데이터와 전략 주입
        mirrorView.Install(mirrorDomain, _rotateMoveStrategy, _mirrorSpriteCatalog.RotateMirrorSprite);

        return mirrorView;
    }

    public MirrorView CreateSlideMirror(int ID, Vector3 position, Quaternion rotation)
    {
        MirrorView mirrorView = _mirrorPool.Get();

        MirrorDomain mirrorDomain = new MirrorDomain();
        mirrorDomain.Init(ID, position, rotation);

        mirrorView.Install(mirrorDomain, _slideMoveStrategy, _mirrorSpriteCatalog.SlideMirrorSprite);

        return mirrorView;
    }
}