using SymphonyFrameWork.CoreSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{

    /// <summary>
    /// �O���b�h�̃}�l�[�W���[�N���X
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private float _gridSize = 1f;

        private List<Vector3> _gridPosList = new();

        private Vector3 _originPosition;

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroySingleton<GridManager>();
        }

        private void Start()
        {
            GridCreate();
        }

        private void GridCreate()
        {
            _gridPosList.Clear();
            var navMeshRange = GetNavMeshCorners();

            for (Vector3 searchPos = navMeshRange.min; searchPos.z <= navMeshRange.max.z; searchPos.z += _gridSize)
            {
                for (searchPos.y = navMeshRange.min.y; searchPos.y <= navMeshRange.max.y + 1; searchPos.y += _gridSize)
                {
                    for (searchPos.x = navMeshRange.min.x; searchPos.x <= navMeshRange.max.x; searchPos.x += _gridSize)
                    {
                        if (NavMesh.SamplePosition(searchPos, out var hit, _gridSize * 0.1f, NavMesh.AllAreas))
                        {
                            _gridPosList.Add(searchPos);
                        }
                    }
                }
            }

            _originPosition = navMeshRange.min;
        }

        private (Vector3 min, Vector3 max) GetNavMeshCorners()
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

            if (triangulation.vertices.Length > 0)
            {
                Vector3 min = triangulation.vertices[0];
                Vector3 max = triangulation.vertices[0];

                // �S���_�����[�v���čŏ��l�ƍő�l���v�Z
                foreach (Vector3 vertex in triangulation.vertices)
                {
                    min = Vector3.Min(min, vertex);
                    max = Vector3.Max(max, vertex);
                }

                //�[����؂�̂Ă�
                const float divisor = 0.5f;
                min = FloorToNearest(min, divisor, _gridSize / 2);
                max = FloorToNearest(max, divisor, _gridSize / 2);

                return (min, max);
            }

            return (Vector3.zero, Vector3.zero);

            //divisor�Ŋ������]���؂�̂Ă�
            Vector3 FloorToNearest(Vector3 vector, float divisor, float offset)
            {
                return new Vector3(
                    Mathf.Floor(vector.x / divisor) * divisor + offset,
                    Mathf.Floor(vector.y / divisor) * divisor,
                    Mathf.Floor(vector.z / divisor) * divisor + offset
                );
            }
        }

        public bool GetGridPosition(Vector3 position, out Vector3 pos)
        {
            //���_����̋���
            Vector3 vector = (position - _originPosition);
            //���_���甼�O���b�h���炷
            vector += new Vector3(_gridSize / 2, _gridSize / 2, _gridSize / 2);
            //�O���b�h���W�n�̃|�W�V�������o��
            vector = new Vector3((int)(vector.x / _gridSize), (int)(vector.y / _gridSize), (int)(vector.z / _gridSize));
            //��ԋ߂��O���b�h�̍��W���o��
            pos =  vector * _gridSize + _originPosition;
            
            //�����ɃO���b�h�����邩�𔻒�
            int index = _gridPosList.IndexOf(pos);
            return 0 <= index;
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
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(
                        center: item + Vector3.up * _gridSize / 3f,
                        size: Vector3.one * (_gridSize * 1f) - Vector3.up * _gridSize / 2f);
                }
            }
        }
#endif
    }
}