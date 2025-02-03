using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{

    /// <summary>
    /// �O���b�h�̃}�l�[�W���[�N���X
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [SerializeField, Tooltip("�O���b�h�̑傫��")]
        private float _gridSize = 1f;

        [Space]

        [SerializeField, Tooltip("�O���b�h�̃v���n�u")]
        private GameObject _gridPrefab;

        [Space]
        [SerializeField, Tooltip("�O���b�h���I�΂�Ă��Ȃ����̃}�e���A��")]
        private Material _unselectGridMaterial;

        [SerializeField, Tooltip("�O���b�h���I�΂�Ă��鎞�̃}�e���A��")]
        private Material _selectGridMaterial;


        private List<GridInfo> _griInfoList = new();

        private GridInfo _highLightingGrid;

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
            List<Vector3> gridPosList = GridCreate();

            if (_gridPrefab)
            {
                GridPrefabInstantiate(gridPosList);
            }
        }


        /// <summary>
        /// NavMesh������ꏊ���������O���b�h�𐶐�
        /// </summary>
        /// <returns>�O���b�h�̍��W�̃��X�g</returns>
        private List<Vector3> GridCreate()
        {
            List<Vector3> list = new();

            var navMeshRange = GetNavMeshCorners();

            for (Vector3 searchPos = navMeshRange.min; searchPos.z <= navMeshRange.max.z; searchPos.z += _gridSize)
            {
                for (searchPos.y = navMeshRange.min.y; searchPos.y <= navMeshRange.max.y + 1; searchPos.y += _gridSize)
                {
                    for (searchPos.x = navMeshRange.min.x; searchPos.x <= navMeshRange.max.x; searchPos.x += _gridSize)
                    {
                        //�T�[�`����ꏊ��NavMesh���������烊�X�g�ɒǉ�
                        if (NavMesh.SamplePosition(searchPos, out _, _gridSize * 0.1f, NavMesh.AllAreas))
                        {
                            list.Add(searchPos);
                        }
                    }
                }
            }

            _originPosition = navMeshRange.min;

            return list;
        }

        /// <summary>
        /// �O���b�h�̈ʒu�Ƀv���n�u�𐶐�
        /// </summary>
        /// <param name="list">�O���b�h�̈ʒu</param>
        private async void GridPrefabInstantiate(List<Vector3> list)
        {
            _griInfoList.Clear();

            //�v���n�u�̐e�I�u�W�F�N�g�𐶐�
            GameObject rootObj = new GameObject("GridPrefabs");
            rootObj.transform.parent = transform;

            //�e�I�u�W�F�N�g�̎q�Ƃ��ăO���b�h�v���n�u���ꊇ����
            GameObject[] objects = await InstantiateAsync(original: _gridPrefab,
                count: list.Count,
                parent: rootObj.transform,
                positions: new Span<Vector3>(list.ToArray()),
                rotations: new Span<Quaternion>(Enumerable.Repeat(Quaternion.identity, list.Count).ToArray()));

            //���������I�u�W�F�N�g��GridInfo���擾
            for (int i = 0; i < objects.Length; i++)
            {
                _griInfoList.Add(
                    objects[i].TryGetComponent<GridInfo>(out var info) ?
                    info : objects[i].AddComponent<GridInfo>()
                    );
            }
        }

        /// <summary>
        /// NavMesh�̒[�̓�_���擾
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// ���͂��ꂽ���W�Ɉ�ԋ߂��O���b�h��̍��W��Ԃ�
        /// </summary>
        /// <param name="position">�������������W</param>
        /// <param name="pos">�O���b�h�̍��W</param>
        /// <returns>�O���b�h�����݂��邩</returns>
        public bool GetGridPosition(Vector3 position, out Vector3 pos)
        {
            return GetGridPosition(position, out pos, out _);
        }

        /// <summary>
        /// ���͂��ꂽ���W�Ɉ�ԋ߂��O���b�h��̍��W��Ԃ�
        /// </summary>
        /// <param name="position">�������������W</param>
        /// <param name="pos">�O���b�h�̍��W</param>
        ///<param name="index">�O���b�h�̃C���f�b�N�X�ԍ�</param>
        /// <returns>�O���b�h�����݂��邩</returns>
        public bool GetGridPosition(Vector3 position, out Vector3 pos, out int index)
        {
            //���_����̋���
            Vector3 vector = (position - _originPosition);
            //���_���甼�O���b�h���炷
            vector += new Vector3(_gridSize / 2, _gridSize / 2, _gridSize / 2);
            //�O���b�h���W�n�̃|�W�V�������o��
            vector = new Vector3((int)(vector.x / _gridSize), (int)(vector.y / _gridSize), (int)(vector.z / _gridSize));
            //��ԋ߂��O���b�h�̍��W���o��
            pos = vector * _gridSize + _originPosition;

            //�����ɃO���b�h�����邩�𔻒�
            index = _griInfoList.Select(gi => gi.transform.position).ToList().IndexOf(pos);
            return 0 <= index;
        }

        /// <summary>
        /// �w�肵���O���b�h���n�C���C�g����
        /// ���͂����X�g�ɂȂ��ꍇ�̓n�C���C�g������
        /// </summary>
        /// <param name="index">�O���b�h�̃C���f�b�N�X�ԍ�</param>
        public void HighLightGrid(int index)
        {
            //�͈͓���������
            if (0 <= index && index < _griInfoList.Count)
            {
                //�O�̃O���b�h�̃n�C���C�g���I�t��
                if (_highLightingGrid != null)
                {
                    HighLightSet(_highLightingGrid, false);
                }

                //�n�C���C�g��\�����L�^
                GridInfo info = _griInfoList[index];

                HighLightSet(info, true);

                _highLightingGrid = info;
            }
            //�͈͊O��������
            else
            {
                //�n�C���C�g���̃O���b�h������΃I�t��
                if (_highLightingGrid != null)
                {
                    HighLightSet(_highLightingGrid, false);

                    _highLightingGrid = null;
                }
            }

            void HighLightSet(GridInfo gridInfo, bool value)
            {
                gridInfo.HighLightSetActive(value);
                gridInfo.GroundMaterialChange(value ? _selectGridMaterial : _unselectGridMaterial);
            }
        }

#if UNITY_EDITOR
        [Header("DebugMode")]
        [SerializeField]
        private bool _gridPosVisible = true;

        private void OnDrawGizmos()
        {
            if (_gridPosVisible && _griInfoList.Count > 0)
            {
                foreach (var item in _griInfoList.Select(gi => gi.transform.position))
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