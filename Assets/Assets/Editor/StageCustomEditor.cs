using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameStageDataSO))]
public class StageCustomEditor : Editor
{
    // GameStageDataSO의 stageID, levelID, mirrorData를 편집할 수 있는 커스텀 인스펙터 구현
    private SerializedProperty _stageIDProp;
    private SerializedProperty _levelIDProp;
    private SerializedProperty _objectDataProp;
    // 프리뷰용 프리팹과 카탈로그
    private static ObjectSpriteCatalogSO _objectSpriteCatalog;
    private static GameObject _rotateMirrorPrefab;
    private static GameObject _slideMirrorPrefab;
    // 프리뷰 오브젝트 관리용 딕셔너리
    private static readonly Dictionary<string, List<GameObject>> PreviewMap = new();
    // 선택된 오브젝트 인덱스
    private int _selectedIndex = -1;
    // 편집 중인 스테이지 데이터
    private GameStageDataSO StageData => (GameStageDataSO)target;

    public void OnEnable() // 시작 세팅
    {
        _stageIDProp = serializedObject.FindProperty("stageID");
        _levelIDProp = serializedObject.FindProperty("levelID");
        _objectDataProp = serializedObject.FindProperty("objectData");

        // SceneView.duringSceneGui: 씬 뷰가 그려질 때마다 실행
        SceneView.duringSceneGui += OnSceneGUI;
        // 프리뷰 오브젝트를 다시 그림(=저장용)
        RebuildPreviewObjects();
    }
    private void OnDisable() // 종료 세팅
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        // 프리뷰 오브젝트 제거
        ClearPreviewObjects();
    }
    
    public override void OnInspectorGUI() // GUI에 수정이 있을 때마다 실행
    {
        // 실제 데이터를 직렬화 필드로 가져옴
        serializedObject.Update();
        // 인스펙터에 데이터 필드 표시
        EditorGUILayout.PropertyField(_stageIDProp);
        EditorGUILayout.PropertyField(_levelIDProp);
        EditorGUILayout.PropertyField(_objectDataProp, true);
        // UI 구분석 간격 및 레이블 설정
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("프리뷰 프리팹", EditorStyles.boldLabel);
        // 실제 데이터 할당 및 할당용 필드 표시
        _objectSpriteCatalog = Resources.Load<ObjectSpriteCatalogSO>("TestData/MirrorCatalogTest");
        _rotateMirrorPrefab = Resources.Load<GameObject>("TestData/MirrorTest");
        _slideMirrorPrefab = Resources.Load<GameObject>("TestData/MirrorTest");

        EditorGUILayout.Space(10);
        DrawToolbar(); // 툴바 버튼 영역

        EditorGUILayout.Space(10);
        DrawObjectList(); // 현재 배치된 데이터 목록 -> 오브젝트 선택, 타입, 위치, 회전 값 수정

        // 수정한 데이터를 실제 데이터에 적용
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 인스펙터의 툴바를 그리는 메서드
    /// </summary>
    private void DrawToolbar()
    {
        // 하위에 정의한 버튼들을 가로로 배치
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotate 추가")) // 버튼 정의
        {
            AddObject(ObjectType.Rotate); // 새 Rotate 타입 오브젝트 데이터 추가
        }
        if (GUILayout.Button("Slide 추가"))
        {
            AddObject(ObjectType.Slide);
        }
        // 버튼 활성화 조건: 오브젝트가 선택된 상태에서만 삭제 버튼 활성화
        GUI.enabled = _selectedIndex >= 0 && _selectedIndex < _objectDataProp.arraySize;
        if (GUILayout.Button("선택 삭제"))
        {
            RemoveSelectedObject(); // 선택된 오브젝트 데이터 삭제
        }
        GUI.enabled = true;

        if (GUILayout.Button("프리뷰 재생성"))
        {
            RebuildPreviewObjects(); // 프리뷰 오브젝트 재생성
            SceneView.RepaintAll(); // 씬 리프레시
        }
        EditorGUILayout.EndHorizontal(); // 가로 배치 종료
    }

    /// <summary>
    /// 실제 데이터의 mirrorData를 보여주고 수정할 수 있는 리스트를 그리는 메서드
    /// </summary>
    private void DrawObjectList()
    {
        EditorGUILayout.LabelField("배치 데이터", EditorStyles.boldLabel); // 섹션 레이블
        if (_objectDataProp == null) // 오류 방어
        {
            EditorGUILayout.HelpBox("mirrorData 프로퍼티를 찾지 못했습니다.", MessageType.Error);
            return;
        }
        // mirrorData 리스트 순회
        for (int i = 0; i < _objectDataProp.arraySize; i++)
        {
            // 리스트의 i번째 데이터 가져오기
            SerializedProperty element = _objectDataProp.GetArrayElementAtIndex(i);
            // 데이터에서 필드 가져오기
            SerializedProperty idProp = element.FindPropertyRelative("ID");
            SerializedProperty typeProp = element.FindPropertyRelative("objectType");
            SerializedProperty posProp = element.FindPropertyRelative("position");
            SerializedProperty rotProp = element.FindPropertyRelative("rotation");

            if (idProp == null || typeProp == null || posProp == null || rotProp == null) // 오류 방어
            {
                EditorGUILayout.HelpBox(
                    "ObjectData 안에 ID / objectType / position / rotation 필드가 있어야 합니다.",
                    MessageType.Error);
                return;
            }
            // 오브젝트를 하나의 UI로 묶어 표시
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal(); // 버튼 가로 배치
            // 버튼에 표시할 문자열 ID와 오브젝트 타입을 숫자로 표시
            string label = $"[{idProp.intValue}] {(ObjectType)typeProp.enumValueIndex}";
            // 버튼 클릭 시 label에 해당되는 오브젝트 선택
            if (GUILayout.Button(label, GUILayout.Width(180)))
            {
                _selectedIndex = i;   
                PingPreviewObject(i); // 선택 오브젝트 강조
                SceneView.RepaintAll(); 
            }
            if (GUILayout.Button("복제", GUILayout.Width(60)))
            {
                DuplicateObject(i); // 선택된 오브젝트 데이터 복제
                return;
            }
            EditorGUILayout.EndHorizontal();

            // 이하의 값들이 바뀌었는지 추적
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(typeProp);
            EditorGUILayout.PropertyField(posProp);
            EditorGUILayout.PropertyField(rotProp);
            // 위의 값이 바뀌면 실행
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties(); // 수정 내용 저장
                SyncPreviewObject(i); // 프리뷰 갱신
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndVertical(); // 여기까지 하나의 UI로 묶어서 표시
        }
    }

    /// <summary>
    /// 오브젝트를 데이터 리스트에 추가
    /// 프리뷰 오브젝트 생성
    /// </summary>
    private void AddObject(ObjectType objectType)
    {
        serializedObject.Update(); // 추가 전 데이터 업데이트

        // 새 오브젝트가 들어갈 인덱스
        int newIndex = _objectDataProp.arraySize;
        _objectDataProp.InsertArrayElementAtIndex(newIndex); // 리스트(변환되어 배열로)에 새 요소 추가

        SerializedProperty newElement = _objectDataProp.GetArrayElementAtIndex(newIndex); // 새 요소 가져오기
        SerializedProperty idProp = newElement.FindPropertyRelative("ID");
        SerializedProperty typeProp = newElement.FindPropertyRelative("objectType");
        SerializedProperty posProp = newElement.FindPropertyRelative("position");
        SerializedProperty rotProp = newElement.FindPropertyRelative("rotation");

        if (idProp == null || typeProp == null || posProp == null || rotProp == null) // 오류 방어
        {
            EditorUtility.DisplayDialog(
                "오류",
                "ObjectData 안에 ID / objectType / Position / Rotation 필드가 있어야 합니다.",
                "확인");
            return;
        }

        // 새 오브젝트용 ID
        idProp.intValue = GenerateNextID();
        // 버튼에서 입력 받은 타입 저장
        typeProp.enumValueIndex = (int)objectType;
        // 새 오브젝트 위치(씬 뷰의 중앙)
        Vector3 spawnPos = GetSceneViewSpawnPosition();
        // 새 오브젝트의 위치 및 로테이션 저장
        posProp.vector3Value = spawnPos;
        rotProp.quaternionValue = Quaternion.identity;
        // 실제 데이터에 적용
        serializedObject.ApplyModifiedProperties();
        // 이 오브젝트에 현재 ID 대입
        _selectedIndex = newIndex;
        // 새 프리뷰 오브젝트 생성
        CreatePreviewObject(newIndex);
        SceneView.RepaintAll();
    }

    /// <summary>
    /// 선택된 오브젝트 삭제
    /// </summary>
    private void RemoveSelectedObject()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _objectDataProp.arraySize) // 오류 방어
            return;

        // 실제 데이터로부터 업데이트
        serializedObject.Update();
        // 선택된 요소를 리스트에서 삭제
        _objectDataProp.DeleteArrayElementAtIndex(_selectedIndex);
        // 실제 데이터에 반영
        serializedObject.ApplyModifiedProperties();
        // 프리뷰 업데이트
        RebuildPreviewObjects();
        // 삭제 후 선택 인덱스 조정
        _selectedIndex = Mathf.Clamp(_selectedIndex - 1, -1, _objectDataProp.arraySize - 1);
        SceneView.RepaintAll();
    }

    /// <summary>
    /// 기존 오브젝트 복제
    /// </summary>
    private void DuplicateObject(int sourceIndex)
    {
        serializedObject.Update(); 
        // 원본 오브젝트 가져오기
        SerializedProperty src = _objectDataProp.GetArrayElementAtIndex(sourceIndex);
        // 복제본의 아이디는 리스트의 끝
        int dstIndex = _objectDataProp.arraySize;
        // 리스트에 새 요소 추가
        _objectDataProp.InsertArrayElementAtIndex(dstIndex);
        // 새 요소 가져오기
        SerializedProperty dst = _objectDataProp.GetArrayElementAtIndex(dstIndex);

        // 새 ID 부여 후, 나머지 데이터는 원본 복사
        dst.FindPropertyRelative("ID").intValue = GenerateNextID();
        dst.FindPropertyRelative("objectType").enumValueIndex = src.FindPropertyRelative("objectType").enumValueIndex;
        dst.FindPropertyRelative("position").vector3Value = src.FindPropertyRelative("position").vector3Value + new Vector3(1f, 0f, 0f);
        dst.FindPropertyRelative("rotation").quaternionValue = src.FindPropertyRelative("rotation").quaternionValue;
        // 실제 데이터에 저장
        serializedObject.ApplyModifiedProperties();
        // 새로 복제된 오브젝트 선택
        _selectedIndex = dstIndex;

        RebuildPreviewObjects(); // 프리뷰 업데이트
        SceneView.RepaintAll();
    }

    /// <summary>
    /// 새 ID 생성
    /// </summary>
    private int GenerateNextID()
    {
        int maxId = -1; // 현재 데이터 리스트에서 가장 큰 ID 찾기
        for (int i = 0; i < _objectDataProp.arraySize; i++)
        {
            SerializedProperty element = _objectDataProp.GetArrayElementAtIndex(i);
            SerializedProperty idProp = element.FindPropertyRelative("ID");
            if (idProp != null && idProp.intValue > maxId)
            {
                maxId = idProp.intValue;
            }
        }
        return maxId + 1;
    }

    /// <summary>
    /// 새 오브젝트 생성 위치 결정
    /// </summary>
    private Vector3 GetSceneViewSpawnPosition()
    {
        if (SceneView.lastActiveSceneView != null) // 현재 활성화된 씬 뷰가 있으면 
        {
            return SceneView.lastActiveSceneView.pivot; // 그 뷰의 중심 위치를 반환
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 선택된 오브젝트 이동, 회전
    /// </summary>
    private void OnSceneGUI(SceneView sceneView)
    {
        if (_selectedIndex < 0) // 오류 방어
            return;

        serializedObject.Update(); // 실제 데이터로부터 업데이트

        if (_selectedIndex >= _objectDataProp.arraySize) // 오류 방어
            return;

        // 선택된 인덱스의 오브젝트 데이터 가져오기
        SerializedProperty element = _objectDataProp.GetArrayElementAtIndex(_selectedIndex);
        SerializedProperty posProp = element.FindPropertyRelative("position");
        SerializedProperty rotProp = element.FindPropertyRelative("rotation");
        SerializedProperty idProp = element.FindPropertyRelative("ID");
        SerializedProperty typeProp = element.FindPropertyRelative("objectType");

        if (posProp == null || rotProp == null || idProp == null || typeProp == null) // 오류 방어
            return;

        // 위치와 로테이션 가져오기
        Vector3 position = posProp.vector3Value; 
        Quaternion rotation = rotProp.quaternionValue;

        // 위치와 로테이션 핸들 표시 및 조작
        EditorGUI.BeginChangeCheck();
        // 위치 핸들과 회전 핸들을 그리기
        Vector3 newPosition = Handles.PositionHandle(position, Quaternion.identity); 
        Quaternion newRotation = Handles.RotationHandle(rotation, position);

        if (EditorGUI.EndChangeCheck()) // 현들 변화 감지
        {
            Undo.RecordObject(StageData, "Move Stage Object"); // 변경 사항 기록 -> undo 지원

            // 새 데이터를 저장
            posProp.vector3Value = newPosition;
            rotProp.quaternionValue = newRotation;
            // 실제 데이터에 저장
            serializedObject.ApplyModifiedProperties();
            // 프리뷰 업데이트
            SyncPreviewObject(_selectedIndex); 
            Repaint();
        }
        // 선택한 오브젝트 위에 ID와 타입 라벨 표시
        Handles.Label(position + Vector3.up * 1.2f, $"ID:{idProp.intValue} / {(ObjectType)typeProp.enumValueIndex}"); 
    }

    /// <summary>
    /// 프리뷰 업데이트
    /// </summary>
    private void RebuildPreviewObjects()
    {
        ClearPreviewObjects(); // 기존 프리뷰 오브젝트 제거

        serializedObject.Update();

        for (int i = 0; i < _objectDataProp.arraySize; i++) // 모든 오브젝트에 대해
        {
            CreatePreviewObject(i); // 프리뷰 오브젝트 생성
        }
    }

    /// <summary>
    /// 프리뷰 오브젝트 생성
    /// </summary>
    private void CreatePreviewObject(int index)
    {
        // 인덱스에 해당하는 데이터 가져오기
        SerializedProperty element = _objectDataProp.GetArrayElementAtIndex(index); 
        SerializedProperty typeProp = element.FindPropertyRelative("objectType");
        SerializedProperty posProp = element.FindPropertyRelative("position");
        SerializedProperty rotProp = element.FindPropertyRelative("rotation");
        SerializedProperty idProp = element.FindPropertyRelative("ID");

        if (typeProp == null || posProp == null || rotProp == null || idProp == null) // 오류 방어
            return;

        GameObject prefab = GetPreviewPrefab((ObjectType)typeProp.enumValueIndex); // 오브젝트 타입에 맞는 프리팹 가져오기
        if (prefab == null) // 오류 방어
            return;

        // 프리팹을 씬에 인스턴스화
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab); 
        instance.name = $"[StagePreview]_{StageData.stageID}_{idProp.intValue}";
        instance.hideFlags = HideFlags.DontSaveInEditor | HideFlags.NotEditable; // 에디터 저장 대상에서 제외 및 편집 불가능 설정(하이어라키에서 편집 불가)
        instance.transform.position = posProp.vector3Value; // 위치 반영
        instance.transform.rotation = rotProp.quaternionValue; // 회전 반영
        SpriteRenderer spriteRenderer = instance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && _objectSpriteCatalog != null)
        {
            ObjectType type = (ObjectType)typeProp.enumValueIndex;

            switch (type)
            {
                case ObjectType.Rotate:
                    spriteRenderer.sprite = _objectSpriteCatalog.RotateMirrorSprite;
                    break;

                case ObjectType.Slide:
                    spriteRenderer.sprite = _objectSpriteCatalog.SlideMirrorSprite;
                    break;
            }
        }
        DisablePreviewBehaviours(instance); // 프리뷰 오브젝트가 런타임 행동을 하지 않도록 관련 컴포넌트 비활성화
        // 현재 스테이지 데이터 관리용 키
        string key = GetPreviewKey(); 

        if (!PreviewMap.ContainsKey(key)) // 해당 키가 없으면 새 리스트 생성
        {
            PreviewMap[key] = new List<GameObject>(); 
        }

        PreviewMap[key].Add(instance); // 프리뷰를 맵에 등록
    }

    /// <summary>
    /// 전체 다시 빌드
    /// </summary>
    /// <param name="index"></param>
    private void SyncPreviewObject(int index)
    {
        RebuildPreviewObjects();
    }

    /// <summary>
    /// 특정 프리뷰 오브젝트 강조
    /// </summary>
    private void PingPreviewObject(int index)
    {
        // 프리뷰 오브젝트 딕셔너리의 키
        string key = GetPreviewKey(); 
        if (!PreviewMap.ContainsKey(key)) // 유효성 검사
            return;

        if (index < 0 || index >= PreviewMap[key].Count) // 유효성 검사
            return;

        // 해당 프리뷰 오브젝트 가져옴
        GameObject go = PreviewMap[key][index]; 
        if (go != null)
        {
            EditorGUIUtility.PingObject(go); // 하이어라키 및 에디터에서 오브젝트 강조
        }
    }

    /// <summary>
    /// 타입에 맞는 프리팹 반환
    /// </summary>
    private GameObject GetPreviewPrefab(ObjectType objectType)
    {
        switch (objectType)
        {
            case ObjectType.Rotate:
                return _rotateMirrorPrefab;

            case ObjectType.Slide:
                return _slideMirrorPrefab;

            default:
                return null;
        }
    }

    /// <summary>
    /// 프리뷰 오브젝트 정리
    /// </summary>
    private void ClearPreviewObjects()
    {
        string key = GetPreviewKey(); // 프리뷰 오브젝트의 딕셔너리 키
        if (!PreviewMap.ContainsKey(key)) // 유효성 검사
            return;

        foreach (GameObject go in PreviewMap[key]) // 해당 키에 등록된 모든 프리뷰 오브젝트 순회
        {
            if (go != null)
            {
                DestroyImmediate(go); // 에디터 모드에서 즉시 오브젝트 제거
            }
        }

        PreviewMap[key].Clear(); // 프리뷰 리스트 제거
        PreviewMap.Remove(key); // 딕셔너리에서 키 제거
    }

    /// <summary>
    /// 현재 편집 중인 스테이지 데이터의 유니티 인스턴스 ID를 키로 사용하여 프리뷰 오브젝트 관리
    /// </summary>
    private string GetPreviewKey()
    {
        return StageData.GetInstanceID().ToString();
    }   

    /// <summary>
    /// 프리뷰 오브젝트를 보이게만 하고, 런타임에 행동하지 않도록 콜라이더, 리지드바디, MonoBehaviour 비활성화
    /// </summary>
    private void DisablePreviewBehaviours(GameObject root)
    {
        Collider2D[] colliders2D = root.GetComponentsInChildren<Collider2D>(true); // 프리뷰 오브젝트의 모든 콜라이더 가져오기(자식 및 비활성화된 오브젝트 포함)
        foreach (Collider2D col in colliders2D)
        {
            col.enabled = false; // 콜라이더 컴포넌트 비활성화
        }

        Rigidbody2D[] rigidbodies2D = root.GetComponentsInChildren<Rigidbody2D>(true);
        foreach (Rigidbody2D rb in rigidbodies2D)
        {
            rb.simulated = false;
        }

        MonoBehaviour[] behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            behaviour.enabled = false;
        }
    }

}
