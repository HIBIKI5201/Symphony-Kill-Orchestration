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
    /// グリッドのマネージャークラス
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        private const string GridPrefabsName = "Grid Prefabs";

        private NavMeshSurface _surface;

        [SerializeField, Tooltip("グリッドの大きさ")]
        private float _gridSize = 1f;

        [Space]

        [SerializeField, Tooltip("グリッドのプレハブ")]
        private GameObject _gridPrefab;

        [Space]
        [SerializeField, Tooltip("グリッドが選ばれていない時のマテリアル")]
        private Material _unselectGridMaterial;

        [SerializeField, Tooltip("グリッドが選ばれている時のマテリアル")]
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
            //ステージが変わるとチャンクを生成するイベントを登録
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += counter => OnStageChanged();

            //初期NavMeshを生成
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
        /// チャンクを生成する
        /// </summary>
        /// <returns></returns>
        public async Task ChunkBuild()
        {
            GameObject chunk = Instantiate(_chunkPrefab, new Vector3(_lastChunkPos, 0, 0), Quaternion.identity);
            chunk.transform.parent = transform;

            _lastChunkPos += 10;

            //アクティブなチャンクのコレクションに追加
            _chunkQueue.Enqueue(chunk.gameObject);
            if (_chunkQueue.Count > 3)
            {
                GameObject obj = _chunkQueue.Dequeue();
                DestroyChunk(obj);
            }

            //NavMeshを再生成
            foreach (GameObject go in _chunkQueue.ToArray())
            {
                NavMeshData navMeshData = _surface.navMeshData;
                AsyncOperation operation = _surface.UpdateNavMesh(navMeshData);

                while (!operation.isDone)
                {
                    await Awaitable.NextFrameAsync();
                }
            }

            //グリッドの位置をリスト化
            List<Vector3> gridPosList = GridCreate();

            gridPosList = FilterNonexistentGrid(gridPosList);

            //グリッドを生成
            if (_gridPrefab)
            {
                await GridPrefabInstantiate(gridPosList, chunk.transform);
            }
        }

        /// <summary>
        /// 指定したチャンクを削除する
        /// </summary>
        /// <param name="chunk"></param>
        private void DestroyChunk(GameObject chunk)
        {
            try
            {
                //チャンクのグリッドを全て取得
                Transform gridPrefabRoot = chunk.transform.Find(GridPrefabsName);
                GridInfo[] positions = gridPrefabRoot.GetComponentsInChildren<GridInfo>().ToArray();

                //削除するチャンクのグリッドをリムーブ
                foreach (GridInfo info in positions)
                {
                    _griInfoList.Remove(info);
                }

                Destroy(chunk);
            }
            catch
            {
                Debug.LogWarning("指定されたチャンクを削除できませんでした");
            }
        }

        /// <summary>
        /// NavMeshがある場所を検索しグリッドを生成
        /// </summary>
        /// <returns>グリッドの座標のリスト</returns>
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
                        //サーチする場所にNavMeshがあったらリストに追加
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
        /// グリッドが既に生成されている位置を除外する
        /// </summary>
        /// <param name="list">フィルターしたい座標のリスト</param>
        /// <returns>フィルター済のリスト</returns>
        private List<Vector3> FilterNonexistentGrid(List<Vector3> list)
        {
            //検索の高速化のためにHashSetに変換
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
        /// グリッドの位置にプレハブを生成
        /// </summary>
        /// <param name="list">グリッドの位置</param>
        private async Task GridPrefabInstantiate(List<Vector3> list, Transform parent)
        {
            GameObject rootObj = new(GridPrefabsName);

            //プレハブの親オブジェクトを生成
            rootObj.transform.parent = parent;


            if (list.Count > 0)
            {
                //親オブジェクトの子としてグリッドプレハブを一括生成
                GameObject[] objects = await InstantiateAsync(original: _gridPrefab,
                    count: list.Count,
                    parent: rootObj.transform,
                    positions: new Span<Vector3>(list.ToArray()),
                    rotations: new Span<Quaternion>(Enumerable.Repeat(Quaternion.identity, list.Count).ToArray()));

                //生成したオブジェクトのGridInfoを取得
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
        /// NavMeshの端の二点を取得
        /// </summary>
        /// <returns></returns>
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

                //端数を切り捨てる
                const float divisor = 0.5f;
                min = FloorToNearest(min, divisor, _gridSize / 2);
                max = FloorToNearest(max, divisor, _gridSize / 2);

                return (min, max);
            }

            return (Vector3.zero, Vector3.zero);

            //divisorで割った余りを切り捨てる
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
        /// 入力された座標に一番近いグリッド上の座標を返す
        /// </summary>
        /// <param name="position">検索したい座標</param>
        /// <param name="pos">グリッドの座標</param>
        ///<param name="index">グリッドのインデックス番号</param>
        /// <returns>グリッドが存在するか</returns>
        public bool GetGridPosition(Vector3 position, out GridInfo info)
        {
            //原点からの距離
            Vector3 vector = (position - _originPosition);
            //原点から半グリッドずらす
            vector += new Vector3(_gridSize / 2, _gridSize / 2, _gridSize / 2);
            //グリッド座標系のポジションを出す
            vector = new Vector3((int)(vector.x / _gridSize), (int)(vector.y / _gridSize), (int)(vector.z / _gridSize));
            //一番近いグリッドの座標を出す
            Vector3 pos = vector * _gridSize + _originPosition;

            //そこにグリッドがあるかを判定
            info = _griInfoList.Find(gi => gi.transform.position == pos);
            return info != null;
        }

        /// <summary>
        /// グリッドが未使用の場合は登録する
        /// 使用されている場合はfalseを返す
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
        /// 使用登録を解除する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryUnregisterGridInfo(GridInfo info)
        {
            return _usedGridList.Remove(info);
        }

        /// <summary>
        /// 指定したグリッドをハイライトする
        /// 入力がリストにない場合はハイライトを消す
        /// </summary>
        /// <param name="index">グリッドのインデックス番号</param>
        public void HighLightGrid(GridInfo info)
        {
            if (info != null)
            {
                //前のグリッドのハイライトをオフに
                if (_highLightingGrid != null)
                {
                    HighLightSet(_highLightingGrid, false);
                }

                //ハイライトを表示し記録
                HighLightSet(info, true);

                _highLightingGrid = info;
            }
            //範囲外だった時
            else
            {
                //ハイライト中のグリッドがあればオフに
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