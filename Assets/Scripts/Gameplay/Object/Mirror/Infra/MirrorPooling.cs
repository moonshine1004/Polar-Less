using UnityEngine;

public interface IMirrorPool<T>
{
    public T Get();
    public void Return(T item);
}

public class MirrorPooling :  IMirrorPool<MirrorView>
{
    public MirrorPooling(MirrorView mirrorPrefab)
    {
        _mirrorPrefab = mirrorPrefab;
        _mirrorPool = new MirrorView[_poolSize];
        for (int i = 0; i < _poolSize; i++)
        {
            _mirrorPool[i] = Object.Instantiate(mirrorPrefab);
            _mirrorPool[i].gameObject.SetActive(false);
        }
    }

    private MirrorView[] _mirrorPool;
    private MirrorView _mirrorPrefab;
    private int _poolSize = 30;



    MirrorView IMirrorPool<MirrorView>.Get()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_mirrorPool[i].gameObject.activeInHierarchy)
            {
                _mirrorPool[i].gameObject.SetActive(true);
                return _mirrorPool[i];
            }
            else
            {
                return Object.Instantiate(_mirrorPrefab);
            }
        }
        return null; 
    }

    public void Return(MirrorView item)
    {
        item.gameObject.SetActive(false);
    }
}