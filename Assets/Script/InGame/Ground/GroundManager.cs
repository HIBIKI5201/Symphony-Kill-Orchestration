using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.InGame
{
    public class GroundManager : MonoBehaviour
    {
        GridManager _gridManager;

        public bool GridInitializeDone { get => _gridManager.IsInitializeDone; }

        [SerializeField]
        private Transform _boundaryLine;
        [SerializeField]
        private float _boundaryLineSpeed = 5;
        private float _firstBoundaryLinePosX;

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
            _gridManager = GetComponent<GridManager>();

            if (!_boundaryLine)
            {
                Debug.LogError("境界線が見つかりません");
            }
        }

        private void Start()
        {
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += MoveBoundaryLine;
        }

        private async void MoveBoundaryLine(int count)
        {
            float nextPosX = count * 10 + _firstBoundaryLinePosX;

            //次のステージ位置に移動するまで繰り返す
            while (nextPosX >= _boundaryLine.position.x)
            {
                _boundaryLine.position += new Vector3(_boundaryLineSpeed * Time.deltaTime, 0, 0);

                await Awaitable.NextFrameAsync();
            }

            //移動完了したら整数値に戻す
            _boundaryLine.position = new Vector3(nextPosX, _boundaryLine.position.y, _boundaryLine.position.z);
        }

        /// <summary>
        /// 入力された座標に一番近いグリッド上の座標を返す
        /// </summary>
        /// <param name="position">検索したい座標</param>
        /// <param name="pos">グリッドの座標</param>
        ///<param name="index">グリッドのインデックス番号</param>
        /// <returns>グリッドが存在するか</returns>
        public bool GetGridPosition(Vector3 position, out GridInfo info) =>
            _gridManager.GetGridPosition(position, out info);

        /// <summary>
        /// グリッドが未使用の場合は登録する
        /// 使用されている場合はfalseを返す
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryRegisterGridInfo(GridInfo info) => _gridManager.TryRegisterGridInfo(info);

        /// <summary>
        /// 使用登録を解除する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryUnregisterGridInfo(GridInfo info) => _gridManager.TryUnregisterGridInfo(info);

        /// <summary>
        /// 指定したグリッドをハイライトする
        /// 入力がリストにない場合はハイライトを消す
        /// </summary>
        /// <param name="index">グリッドのインデックス番号</param>
        public void HighLightGrid(GridInfo info) => _gridManager.HighLightGrid(info);
    }
}
