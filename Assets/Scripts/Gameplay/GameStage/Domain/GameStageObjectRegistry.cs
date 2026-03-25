using System.Collections.Generic;

public interface IRegistMirror
{
    void RegisterMirror(MirrorView mirrorView);
    List<MirrorView> GetActivedMirrorList();
}

public class GameStageObjectRegistry : IRegistMirror
{    
    private readonly List<MirrorView> _activedMirror = new List<MirrorView>();

    public void RegisterMirror(MirrorView mirrorView)
    {
        _activedMirror.Add(mirrorView);
    }

    public List<MirrorView> GetActivedMirrorList()
    {
        return _activedMirror;
    }

    
}