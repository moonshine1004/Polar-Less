using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IMirrorPool<T>
{
    public T Get();
    public void Return(T item);
}

public class MirrorPooling : MonoBehaviour, IMirrorPool<MirrorBaseView>, IMirrorPool<RotateMirrorView>
{
    [SerializeField] private MirrorBaseView _baseMirrorPrefab;
    [SerializeField] private RotateMirrorView _rotateMirrorPrefab;
    private MirrorBaseView[] _baseMirrorPool;
    private RotateMirrorView[] _rotateMirrorPool;

    private EventBus _mirrorViewEventBus;

    private MirrorInstallUseCase _mirrorInstallUseCase;

    private int _poolSize = 10;

    public void Install(EventBus mirrorViewEventBus)
    {
        _mirrorViewEventBus = mirrorViewEventBus;
    }

    #region Unity Lifecycle
    private void Awake()
    {
        _baseMirrorPool = new MirrorBaseView[_poolSize];
        _rotateMirrorPool = new RotateMirrorView[_poolSize];

        for (int i = 0; i < _poolSize; i++)
        {
            _baseMirrorPool[i] = Instantiate(_baseMirrorPrefab, transform);
            _baseMirrorPool[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _poolSize; i++)
        {
            _rotateMirrorPool[i] = Instantiate(_rotateMirrorPrefab, transform);
            _rotateMirrorPool[i].gameObject.SetActive(false);
        }
    }
    #endregion

    MirrorBaseView IMirrorPool<MirrorBaseView>.Get()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_baseMirrorPool[i].gameObject.activeInHierarchy)
            {
                _baseMirrorPool[i].InstallMirror(_mirrorInstallUseCase.Excute());
                _baseMirrorPool[i].gameObject.SetActive(true);
                return _baseMirrorPool[i];
            }
        }
        return null; 
    }

    public void Return(MirrorBaseView item)
    {
        item.gameObject.SetActive(false);
    }

    RotateMirrorView IMirrorPool<RotateMirrorView>.Get()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_rotateMirrorPool[i].gameObject.activeInHierarchy)
            {
                _rotateMirrorPool[i].InstallMirror(_mirrorInstallUseCase.Excute());
                _rotateMirrorPool[i].gameObject.SetActive(true);
                return _rotateMirrorPool[i];
            }
        }
        return null; 
    }

    public void Return(RotateMirrorView item)
    {
        throw new NotImplementedException();
    }
}