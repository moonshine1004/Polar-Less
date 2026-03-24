using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameStageDataSO))]
public class StageCustomEditor : Editor
{
    // GameStageDataSO의 stageID, levelID, mirrorData를 편집할 수 있는 커스텀 인스펙터 구현
    private SerializedProperty _stageIDProp;
    private SerializedProperty _levelIDProp;
    private SerializedProperty _mirrorDataProp;
    // 프리뷰용 프리팹과 카탈로그
    private static ObjectSpriteCatalogSO _objectSpriteCatalog;
    private static GameObject _rotateMirrorPrefab;
    private static GameObject _slideMirrorPrefab;
    // 프리뷰 오브젝트 관리용 딕셔너리
    private static readonly Dictionary<string, List<GameObject>> PreviewMap = new();
    // 선택된 오브젝트 인덱스
    private int _selectedIndex = 0;
    // 편집 중인 스테이지 데이터
    private GameStageDataSO StageData => (GameStageDataSO)target;

    public void OnEnable() // 시작 세팅
    {
        _stageIDProp = serializedObject.FindProperty("stageID");
        _levelIDProp = serializedObject.FindProperty("levelID");
        _mirrorDataProp = serializedObject.FindProperty("mirrorData");

        // SceneView.duringSceneGui: 씬 뷰가 그려질 때마다 실행
        // SceneView.duringSceneGui += OnSceneGUI;
        // 프리뷰 오브젝트를 다시 그림(=저장용)
        // RebuildPreviewObjects();
    }
    private void OnDisable() // 종료 세팅
    {
        // SceneView.duringSceneGui -= OnSceneGUI;
        // 프리뷰 오브젝트 제거
        // ClearPreviewObjects();
    }
    
    public override void OnInspectorGUI() // GUI에 수정이 있을 때마다 실행
    {
        // 실제 데이터를 직렬화 필드로 가져옴
        serializedObject.Update();
        // 인스펙터에 데이터 필드 표시
        EditorGUILayout.PropertyField(_stageIDProp);
        EditorGUILayout.PropertyField(_levelIDProp);
        EditorGUILayout.PropertyField(_mirrorDataProp, true);
        // UI 구분석 간격 및 레이블 설정
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("프리뷰 프리팹", EditorStyles.boldLabel);
        // 실제 데이터 할당 및 할당용 필드 표시
        _objectSpriteCatalog = (ObjectSpriteCatalogSO)EditorGUILayout.ObjectField("오브젝트 스프라이트 카탈로그", _objectSpriteCatalog, typeof(ObjectSpriteCatalogSO), false);
        _rotateMirrorPrefab.GetComponent<SpriteRenderer>().sprite = _objectSpriteCatalog.RotateMirrorSprite;
        _slideMirrorPrefab.GetComponent<SpriteRenderer>().sprite = _objectSpriteCatalog.SlideMirrorSprite;

        EditorGUILayout.Space(10);
        DrawToolbar(); // 툴바 버튼 영역

        EditorGUILayout.Space(10);
        // DrawObjectList(); // 현재 배치된 데이터 목록 -> 오브젝트 선택, 타입, 위치, 회전 값 수정

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
            //AddObject(ObjectType.Rotate); // 새 Rotate 타입 오브젝트 데이터 추가
        }
        if (GUILayout.Button("Slide 추가"))
        {
            //AddObject(ObjectType.Slide);
        }
        // 버튼 활성화 조건: 오브젝트가 선택된 상태에서만 삭제 버튼 활성화
        GUI.enabled = _selectedIndex >= 0 && _selectedIndex < _mirrorDataProp.arraySize;
        if (GUILayout.Button("선택 삭제"))
        {
            //RemoveSelectedObject(); // 선택된 오브젝트 데이터 삭제
        }
        GUI.enabled = true;

        if (GUILayout.Button("프리뷰 재생성"))
        {
            //RebuildPreviewObjects(); // 프리뷰 오브젝트 재생성
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
        if (_mirrorDataProp == null) // 오류 방어
        {
            EditorGUILayout.HelpBox("mirrorData 프로퍼티를 찾지 못했습니다.", MessageType.Error);
            return;
        }
        // mirrorData 리스트 순회
        for (int i = 0; i < _mirrorDataProp.arraySize; i++)
        {
            // 리스트의 i번째 데이터 가져오기
            SerializedProperty element = _mirrorDataProp.GetArrayElementAtIndex(i);
            // 데이터에서 필드 가져오기
            SerializedProperty idProp = element.FindPropertyRelative("ID");
            SerializedProperty typeProp = element.FindPropertyRelative("objectType");
            SerializedProperty posProp = element.FindPropertyRelative("Position");
            SerializedProperty rotProp = element.FindPropertyRelative("Rotation");

            if (idProp == null || typeProp == null || posProp == null || rotProp == null) // 오류 방어
            {
                EditorGUILayout.HelpBox(
                    "ObjectData 안에 ID / objectType / Position / Rotation 필드가 있어야 합니다.",
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
        int newIndex = _mirrorDataProp.arraySize;
        _mirrorDataProp.InsertArrayElementAtIndex(newIndex);

        SerializedProperty newElement = _mirrorDataProp.GetArrayElementAtIndex(newIndex);

        SerializedProperty idProp = newElement.FindPropertyRelative("ID");
        SerializedProperty typeProp = newElement.FindPropertyRelative("objectType");
        SerializedProperty posProp = newElement.FindPropertyRelative("Position");
        SerializedProperty rotProp = newElement.FindPropertyRelative("Rotation");

        if (idProp == null || typeProp == null || posProp == null || rotProp == null)
        {
            EditorUtility.DisplayDialog(
                "오류",
                "ObjectData 안에 ID / objectType / Position / Rotation 필드가 있어야 합니다.",
                "확인");
            return;
        }

        idProp.intValue = GenerateNextID();
        typeProp.enumValueIndex = (int)objectType;

        Vector3 spawnPos = GetSceneViewSpawnPosition();
        posProp.vector3Value = spawnPos;
        rotProp.quaternionValue = Quaternion.identity;

        serializedObject.ApplyModifiedProperties();

        _selectedIndex = newIndex;
        CreatePreviewObject(newIndex);
        SceneView.RepaintAll();
    }
}
