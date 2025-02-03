using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{

    /// <summary>
    /// グリッドのマネージャークラス
    /// </summary>
    public class GridManager : MonoBehaviour
    {
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
        /// グリッドの位置にプレハブを生成
        /// </summary>
        /// <param name="list">グリッドの位置</param>
        private async void GridPrefabInstantiate(List<Vector3> list)
        {
            _griInfoList.Clear();

            //プレハブの親オブジェクトを生成
            GameObject rootObj = new GameObject("GridPrefabs");
            rootObj.transform.parent = transform;

            //親オブジェクトの子としてグリッドプレハブを一括生成
            GameObject[] objects = await InstantiateAsync(original: _gridPrefab,
                count: list.Count,
                parent: rootObj.transform,
                positions: new Span<Vector3>(list.ToArray()),
                rotations: new Span<Quaternion>(Enumerable.Repeat(Quaternion.identity, list.Count).ToArray()));

            //生成したオブジェクトのGridInfoを取得
            for (int i = 0; i < objects.Length; i++)
            {
                _griInfoList.Add(
                    objects[i].TryGetComponent<GridInfo>(out var info) ?
                    info : objects[i].AddComponent<GridInfo>()
                    );
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
        /// <returns>グリッドが存在するか</returns>
        public bool GetGridPosition(Vector3 position, out Vector3 pos)
        {
            return GetGridPosition(position, out pos, out _);
        }

        /// <summary>
        /// 入力された座標に一番近いグリッド上の座標を返す
        /// </summary>
        /// <param name="position">検索したい座標</param>
        /// <param name="pos">グリッドの座標</param>
        ///<param name="index">グリッドのインデックス番号</param>
        /// <returns>グリッドが存在するか</returns>
        public bool GetGridPosition(Vector3 position, out Vector3 pos, out int index)
        {
            //原点からの距離
            Vector3 vector = (position - _originPosition);
            //原点から半グリッドずらす
            vector += new Vector3(_gridSize / 2, _gridSize / 2, _gridSize / 2);
            //グリッド座標系のポジションを出す
            vector = new Vector3((int)(vector.x / _gridSize), (int)(vector.y / _gridSize), (int)(vector.z / _gridSize));
            //一番近いグリッドの座標を出す
            pos = vector * _gridSize + _originPosition;

            //そこにグリッドがあるかを判定
            index = _griInfoList.Select(gi => gi.transform.position).ToList().IndexOf(pos);
            return 0 <= index;
        }

        /// <summary>
        /// 指定したグリッドをハイライトする
        /// 入力がリストにない場合はハイライトを消す
        /// </summary>
        /// <param name="index">グリッドのインデックス番号</param>
        public void HighLightGrid(int index)
        {
            //範囲内だった時
            if (0 <= index && index < _griInfoList.Count)
            {
                //前のグリッドのハイライトをオフに
                if (_highLightingGrid != null)
                {
                    HighLightSet(_highLightingGrid, false);
                }

                //ハイライトを表示し記録
                GridInfo info = _griInfoList[index];

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