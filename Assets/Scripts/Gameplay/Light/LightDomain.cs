using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Light.Domain
{
    public class LightDomain
    {
        private List<Vector2> _lightPath = new List<Vector2>(); // 레이저의 경로를 저장하는 리스트
        private int _pathIndex = 0;
        private float _maxDistance = 18f; 
        private int _maxReflections = 30;

        public List<Vector2> LightPath
        {
            get => _lightPath;
            set => _lightPath = value;
        }
        public int PathIndex
        {
            get => _pathIndex;
            set => _pathIndex = value;
        }
        public float MaxDistance => _maxDistance;
        public int MaxReflections => _maxReflections;
        
        /// <summary>
        /// N번째 거울이 움직일 때
        /// </summary>
        public void OnMirrorNMovw(int n)
        {
            _lightPath.RemoveRange(n, _lightPath.Count - n);
        }

    }
}