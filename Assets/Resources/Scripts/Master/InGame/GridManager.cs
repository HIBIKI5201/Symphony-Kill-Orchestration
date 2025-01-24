using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{

    /// <summary>
    /// グリッドのマネージャークラス
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private float _gridSize = 1f;

        private List<Vector3> _gridPosList = new();

        private void Start()
        {
            GridCreate();
        }

        private void GridCreate()
        {
            _gridPosList.Clear();
            var navMeshRange = GetNavMeshCorners();
            Vector3 searchPos = navMeshRange.min;

            for (; searchPos.z <= navMeshRange.max.z; searchPos.z++)
            {
                for (searchPos.y = navMeshRange.min.y ; searchPos.y <= navMeshRange.max.y + 1; searchPos.y++)
                {
                    for (searchPos.x = navMeshRange.min.x; searchPos.x <= navMeshRange.max.x; searchPos.x++)
                    {
                        if (NavMesh.SamplePosition(searchPos, out var hit, _gridSize * 0.1f, NavMesh.AllAreas))
                        {
                            _gridPosList.Add(searchPos);
                        }
                    }
                }
            }
        }

        private (Vector3 min, Vector3 max) GetNavMeshCorners()
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

            if (triangulation.vertices.Length > 0)
            {
                Vector3 min = triangulation.vertices[0];
                Vector3 max = triangulation.vertices[0];

                // 全頂点をループして最小値と最大値を計算
                foreach (Vector3 vertex in triangulation.vertices)
                {
                    min = Vector3.Min(min, vertex);
                    max = Vector3.Max(max, vertex);
                }

                return (min, max);
            }

            return (Vector3.zero, Vector3.zero);
        }

#if UNITY_EDITOR
        [Header("DebugMode")]
        [SerializeField]
        private bool _gridPosVisible = true;

        private void OnDrawGizmos()
        {
            if (_gridPosVisible && _gridPosList.Count > 0)
            {
                foreach (var item in _gridPosList)
                {
                    Gizmos.color =  Color.green;
                    Gizmos.DrawWireCube(item + Vector3.up * _gridSize / 3f,
                        Vector3.one * (_gridSize * 0.9f) - Vector3.up * _gridSize / 2f);
                }
            }
        }
#endif
    }
}