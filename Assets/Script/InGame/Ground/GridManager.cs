using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{
    /// <summary>
    /// �O���b�h�̃}�l�[�W���[�N���X
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        private const string GridPrefabsName = "Grid Prefabs";

        private NavMeshSurface _surface;

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

        private List<GridInfo> _usedGridList = new();

        [Space]

        [SerializeField]
        private GameObject _chunkPrefab;

        private Queue<GameObject> _chunkQueue = new();

        private readonly object _chunkCreatLock = new object();

        private float _lastChunkPos = 0;

        public bool IsInitializeDone { get => _griInfoList.Count > 0; }

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        private void Awake()
        {
            _surface = GetComponent<NavMeshSurface>();
        }

        private async void Start()
        {
            //�X�e�[�W���ς��ƃ`�����N�𐶐�����C�x���g��o�^
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += counter => OnStageChanged();

            //����NavMesh�𐶐�
            _surface.BuildNavMesh();

            for (int i = 0; i < 3; i++)
            {
                await ChunkBuild();
            }


            async void OnStageChanged()
            {
                await ChunkBuild();
            }
        }


        [ContextMenu("New Chunk Create")]
        /// <summary>
        /// �`�����N�𐶐�����
        /// </summary>
        /// <returns></returns>
        public async Task ChunkBuild()
        {
            GameObject chunk = Instantiate(_chunkPrefab, new Vector3(_lastChunkPos, 0, 0), Quaternion.identity);
            chunk.transform.parent = transform;

            _lastChunkPos += 10;

            //�A�N�e�B�u�ȃ`�����N�̃R���N�V�����ɒǉ�
            _chunkQueue.Enqueue(chunk.gameObject);
            if (_chunkQueue.Count > 3)
            {
                GameObject obj = _chunkQueue.Dequeue();
                DestroyChunk(obj);
            }

            //NavMesh���Đ���
            foreach (GameObject go in _chunkQueue.ToArray())
            {
                NavMeshData navMeshData = _surface.navMeshData;
                AsyncOperation operation = _surface.UpdateNavMesh(navMeshData);

                while (!operation.isDone)
                {
                    await Awaitable.NextFrameAsync();
                }
            }

            //�O���b�h�̈ʒu�����X�g��
            List<Vector3> gridPosList = GridCreate();

            gridPosList = FilterNonexistentGrid(gridPosList);

            //�O���b�h�𐶐�
            if (_gridPrefab)
            {
                await GridPrefabInstantiate(gridPosList, chunk.transform);
            }
        }

        /// <summary>
        /// �w�肵���`�����N���폜����
        /// </summary>
        /// <param name="chunk"></param>
        private void DestroyChunk(GameObject chunk)
        {
            try
            {
                //�`�����N�̃O���b�h��S�Ď擾
                Transform gridPrefabRoot = chunk.transform.Find(GridPrefabsName);
                GridInfo[] positions = gridPrefabRoot.GetComponentsInChildren<GridInfo>().ToArray();

                //�폜����`�����N�̃O���b�h�������[�u
                foreach (GridInfo info in positions)
                {
                    _griInfoList.Remove(info);
                }

                Destroy(chunk);
            }
            catch
            {
                Debug.LogWarning("�w�肳�ꂽ�`�����N���폜�ł��܂���ł���");
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
        /// �O���b�h�����ɐ�������Ă���ʒu�����O����
        /// </summary>
        /// <param name="list">�t�B���^�[���������W�̃��X�g</param>
        /// <returns>�t�B���^�[�ς̃��X�g</returns>
        private List<Vector3> FilterNonexistentGrid(List<Vector3> list)
        {
            //�����̍������̂��߂�HashSet�ɕϊ�
            HashSet<Vector3> filter = new(_griInfoList.Select(gi => gi.transform.position));

            List<Vector3> filtered = new();

            foreach (var pos in list)
            {
                if (!filter.Contains(pos))
                {
                    filtered.Add(pos);
                }
            }

            return filtered;
        }

        /// <summary>
        /// �O���b�h�̈ʒu�Ƀv���n�u�𐶐�
        /// </summary>
        /// <param name="list">�O���b�h�̈ʒu</param>
        private async Task GridPrefabInstantiate(List<Vector3> list, Transform parent)
        {
            GameObject rootObj = new(GridPrefabsName);

            //�v���n�u�̐e�I�u�W�F�N�g�𐶐�
            rootObj.transform.parent = parent;


            if (list.Count > 0)
            {
                //�e�I�u�W�F�N�g�̎q�Ƃ��ăO���b�h�v���n�u���ꊇ����
                GameObject[] objects = await InstantiateAsync(original: _gridPrefab,
                    count: list.Count,
                    parent: rootObj.transform,
                    positions: new Span<Vector3>(list.ToArray()),
                    rotations: new Span<Quaternion>(Enumerable.Repeat(Quaternion.identity, list.Count).ToArray()));

                //���������I�u�W�F�N�g��GridInfo���擾
                for (int i = 0; i < objects.Length; i++)
                {
                    GameObject obj = objects[i];
                    _griInfoList.Add(
                        obj.TryGetComponent<GridInfo>(out var info) ?
                        info : obj.AddComponent<GridInfo>()
                        );
                    obj.transform.localScale = Vector3.one * _gridSize;
                }
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
        ///<param name="index">�O���b�h�̃C���f�b�N�X�ԍ�</param>
        /// <returns>�O���b�h�����݂��邩</returns>
        public bool GetGridPosition(Vector3 position, out GridInfo info)
        {
            //���_����̋���
            Vector3 vector = (position - _originPosition);
            //���_���甼�O���b�h���炷
            vector += new Vector3(_gridSize / 2, _gridSize / 2, _gridSize / 2);
            //�O���b�h���W�n�̃|�W�V�������o��
            vector = new Vector3((int)(vector.x / _gridSize), (int)(vector.y / _gridSize), (int)(vector.z / _gridSize));
            //��ԋ߂��O���b�h�̍��W���o��
            Vector3 pos = vector * _gridSize + _originPosition;

            //�����ɃO���b�h�����邩�𔻒�
            info = _griInfoList.Find(gi => gi.transform.position == pos);
            return info != null;
        }

        /// <summary>
        /// �O���b�h�����g�p�̏ꍇ�͓o�^����
        /// �g�p����Ă���ꍇ��false��Ԃ�
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryRegisterGridInfo(GridInfo info)
        {
            if (!_usedGridList.Contains(info))
            {
                _usedGridList.Add(info);
                return true;
            }

            return false;
        }

        /// <summary>
        /// �g�p�o�^����������
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryUnregisterGridInfo(GridInfo info)
        {
            return _usedGridList.Remove(info);
        }

        /// <summary>
        /// �w�肵���O���b�h���n�C���C�g����
        /// ���͂����X�g�ɂȂ��ꍇ�̓n�C���C�g������
        /// </summary>
        /// <param name="index">�O���b�h�̃C���f�b�N�X�ԍ�</param>
        public void HighLightGrid(GridInfo info)
        {
            if (info != null)
            {
                //�O�̃O���b�h�̃n�C���C�g���I�t��
                if (_highLightingGrid != null)
                {
                    HighLightSet(_highLightingGrid, false);
                }

                //�n�C���C�g��\�����L�^
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