using System;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorPooling : MonoBehaviour
{
    [SerializeField] private MirrorBaseView _baseMirrorPrefab;
    [SerializeField] private RotateMirrorView _rotateMirrorPrefab;
    private MirrorBaseView[] _baseMirrorPool;
    private MirrorBaseView[] _rotateMirrorPool;

    private int _poolSize = 10;

    #region Unity Lifecycle
    private void Awake()
    {
        _baseMirrorPool = new MirrorBaseView[_poolSize];
        _rotateMirrorPool = new RotateMirrorView[_poolSize];

        for (int i = 0; i < _poolSize; i++)
        {
            _baseMirrorPool[i] = Instantiate(_baseMirrorPrefab, transform);
            _baseMirrorPool[i].gameObject.SetActive(false);
            _baseMirrorPool[i].MirrorID = i;
        }
        for (int i = 0; i < _poolSize; i++)
        {
            _rotateMirrorPool[i] = Instantiate(_rotateMirrorPrefab, transform);
            _rotateMirrorPool[i].gameObject.SetActive(false);
            _rotateMirrorPool[i].MirrorID = i;
        }
    }
    #endregion

    public MirrorBaseView GetBaseMirror()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            if (!_baseMirrorPool[i].gameObject.activeInHierarchy)
            {
                return _baseMirrorPool[i];
            }
        }
        return null; 
    }
}