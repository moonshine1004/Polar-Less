using System;
using UnityEngine;

[Serializable]
public struct ParallaxLayer
{
    [Tooltip("이 레이어를 실제로 움직일 Transform")]
    public Transform layerTransform;

    [Tooltip("0이면 고정, 1이면 카메라와 동일 속도.")]
    public float parallaxFactor;
}

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera; 
    [SerializeField] private ParallaxLayer[] _layers; 

    private Vector3 _startCameraPosition; 
    private Vector3[] _startLayerPositions; 

    private void Awake()
    {
        // 타겟 카메라가 없으면 메인 카메라를 타겟 카메라로 설정
        if (_targetCamera == null)
            _targetCamera = Camera.main;
    }

    private void Start()
    {
        _startCameraPosition = _targetCamera != null ? _targetCamera.transform.position : Vector3.zero; // 타겟 카메라가 있으면 타겟 카메라의 위치를 카메라 시작 위치로 설정, 없으면 0,0,0을 설정

        _startLayerPositions = new Vector3[_layers.Length]; // 레이어 시작 위치 배열 초기화
        for (int i = 0; i < _layers.Length; i++)
        {
            _startLayerPositions[i] = _layers[i].layerTransform != null // 레이어 트랜스폼이 있으면 레이어 트랜스폼의 위치를 레이어 시작 위치로 설정, 없으면 0,0,0을 설정
                ? _layers[i].layerTransform.position
                : Vector3.zero;
        }
    }

    private void LateUpdate() // LateUpdate에서 처리하여 카메라 이동 후에 레이어 위치를 업데이트하도록 합니다.
    {
        if (_targetCamera == null)
            return; // 타겟 카메라가 없으면 종료

        Vector3 cameraDelta = _targetCamera.transform.position - _startCameraPosition; // 카메라 시작 위치와 타겟 카메라의 위치의 차이를 계산

        // 변위가 거의 없으면 불필요한 Set을 줄임
        if (cameraDelta.sqrMagnitude < 0.000001f)
            return;

        for (int i = 0; i < _layers.Length; i++)
        {
            var layer = _layers[i];
            if (layer.layerTransform == null)
                continue; // 레이어 트랜스폼이 없으면 다음 레이어로 이동

            Vector3 startPos = _startLayerPositions[i];

            // 여기서는 X축 페럴랙스만 적용합니다. Y축이나 Z축도 적용하려면 아래 계산식을 수정하면 됩니다.
            Vector3 nextPos = startPos + new Vector3(cameraDelta.x * layer.parallaxFactor, 0f, 0f); 

            layer.layerTransform.position = nextPos; // 레이어 트랜스폼의 위치를 다음 레이어 위치로 설정  
        }
    }
}

