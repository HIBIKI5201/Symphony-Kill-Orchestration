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
        public float GridSize { get => _gridSize; }

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

        private Vector3? _originPosition;

        public Vector3 Edge { get => _originPosition.Value - new Vector3(_gridSize / 2, 0, _gridSize / 2); }

        private List<GridInfo> _usedGridList = new();

        [Space]

        [SerializeField]
        private GameObject _normalChunkPrefab;

        [SerializeField]
        private List<GameObject> _enemyChunkPrefabList = new();

        private Queue<GameObject> _chunkQueue = new();

        private readonly object _chunkCreatLock = new object();

        private float _lastChunkPos = 0;

        public bool IsInitializeDone { get; private set; }

        private void Awake()
        {
            _surface = GetComponent<NavMeshSurface>();

            _lastChunkPos = -GroundManager.ChunkSize;
            IsInitializeDone = false;
        }

        private async void Start()
        {
            //ステージが変わるとチャンクを生成するイベントを登録
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += counter => OnStageChanged();

            //初期NavMeshを生成
            _surface.BuildNavMesh();

            for (int i = 0; i < GroundManager.ChunkCapacity; i++)
            {
                await ChunkBuild(_normalChunkPrefab);
            }

            IsInitializeDone = true;
        }

        [ContextMenu("OnStageChangedCreateChunk")]
        private async void OnStageChanged()
        {
            IsInitializeDone = false;

            //エネミーチャンクからランダムに取得
            int index = UnityEngine.Random.Range(0, _enemyChunkPrefabList.Count);
            GameObject chunk = _enemyChunkPrefabList[index];

            await ChunkBuild(chunk);

            IsInitializeDone = true;
        }

        /// <summary>
        /// チャンクを生成する
        /// </summary>
        /// <returns></returns>
        public async Task ChunkBuild(GameObject chunkPrefab)
        {
            //ランダムな回転を加える
            float randomRotation = UnityEngine.Random.Range(0, 4) * 90;

            //チャンクを生成
            GameObject chunk = Instantiate(chunkPrefab,
                new Vector3(_lastChunkPos, 0, 0),
                Quaternion.Euler(new Vector3(0, randomRotation, 0) + chunkPrefab.transform.eulerAngles));

            chunk.transform.parent = transform;

            _lastChunkPos += GroundManager.ChunkSize;

            //アクティブなチャンクのコレクションに追加
            _chunkQueue.Enqueue(chunk.gameObject);

            //NavMeshを再生成
            await _surface.UpdateNavMesh(_surface.navMeshData);

            //グリッドの位置をリスト化
            List<Vector3> gridPosList = GridCreate();

            gridPosList = FilterNonexistentGrid(gridPosList);

            //グリッドを生成
            if (_gridPrefab)
            {
                await GridPrefabInstantiate(gridPosList, chunk.transform);
            }

            //キャパシティ以上のチャンクを破壊
            //（チャンク生成後に破壊しないと新しいチャンクにこのチャンクのグリッドが生成される）
            if (_chunkQueue.Count > GroundManager.ChunkCapacity)
            {
                GameObject obj = _chunkQueue.Dequeue();
                DestroyChunk(obj);

                await _surface.UpdateNavMesh(_surface.navMeshData);
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

            if (_originPosition == null)
            {
                _originPosition = navMeshRange.min;
            }

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
            HashSet<Vector3Int> filter = new(_griInfoList.Select(gi => gi.Position));

            List<Vector3> filtered = new();

            foreach (var pos in list)
            {
                Vector3Int p = Vector3Int.FloorToInt(pos - _originPosition.Value);

                if (!filter.Contains(p))
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
            //プレハブの親オブジェクトを生成
            GameObject rootObj = new(GridPrefabsName);
            rootObj.transform.SetParent(parent);
            rootObj.transform.localPosition = Vector3.zero;
            rootObj.transform.localRotation = Quaternion.Euler(Vector3.zero);

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
                    GridInfo info = obj.GetComponent<GridInfo>();
                    info.Init(Edge, _gridSize);
                    _griInfoList.Add(info);
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
        public bool GetGridByPosition(Vector3 position, out GridInfo info)
        {
            Vector3Int pos = Vector3Int.FloorToInt(position - Edge + new Vector3(0, 0.3f, 0)); //少し上方向を優先する

            //そこにグリッドがあるかを判定
            info = _griInfoList.Find(gi => gi.Position == pos);
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
        public bool TryUnregisterGridInfo(GridInfo info) => _usedGridList.Remove(info);

        /// <summary>
        /// グリッドが使用済みに登録されているかどうか
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsRegisterGridInfo(GridInfo info) => _usedGridList.Contains(info);

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