using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.InGame
{
    public class GroundManager : MonoBehaviour
    {
        GridManager _gridManager;

        public bool GridInitializeDone { get => _gridManager.IsInitializeDone; }

        public float GridSize { get => _gridManager.GridSize; }

        [SerializeField]
        private Transform _overBoundaryLine;
        [SerializeField]
        private Transform _nextBoundaryLine;

        private float _firstBoundaryLineX;
        public float FirstBoundaryLineX { get => _firstBoundaryLineX; }

        [SerializeField]
        private float _boundaryLineSpeed = 5;

        public const int NextBoundaryLineCount = 3;
        public const int ChunkCapacity = 5;
        public const float ChunkSize = 10;

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

            if (_overBoundaryLine)
            {
                _firstBoundaryLineX = _overBoundaryLine.position.x;
            }
            else
            {
                Debug.LogError("???E??????????܂???");
            }
        }

        private void Start()
        {
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += MoveBoundaryLine;
        }

        /// <summary>
        /// ???E????ړ???????
        /// </summary>
        /// <param name="count"></param>
        private async void MoveBoundaryLine(int count)
        {
            float nextPosX = count * ChunkSize + _firstBoundaryLineX;

            //???̃X?e?[?W?ʒu?Ɉړ?????܂ŌJ??Ԃ?
            while (nextPosX >= _overBoundaryLine.position.x)
            {
                _overBoundaryLine.position += new Vector3(_boundaryLineSpeed * Time.deltaTime, 0, 0);
                _nextBoundaryLine.position += new Vector3(_boundaryLineSpeed * Time.deltaTime, 0, 0);

                try
                {
                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                }
                catch { }
            }

            //?ړ??????????琮???l?ɖ߂?
            _overBoundaryLine.position = new Vector3(nextPosX, _overBoundaryLine.position.y, _overBoundaryLine.position.z);
            _nextBoundaryLine.position = new Vector3(nextPosX + ChunkSize * NextBoundaryLineCount, _nextBoundaryLine.position.y, _nextBoundaryLine.position.z);
        }

        /// <summary>
        /// ???͂??ꂽ???W?Ɉ?ԋ߂??O???b?h??̍??W??Ԃ?
        /// </summary>
        /// <param name="position">?????????????W</param>
        /// <param name="pos">?O???b?h?̍??W</param>
        ///<param name="index">?O???b?h?̃C???f?b?N?X?ԍ?</param>
        /// <returns>?O???b?h?????݂??邩</returns>
        public bool GetGridByPosition(Vector3 position, out GridInfo info) =>
            _gridManager.GetGridByPosition(position, out info);

        /// <summary>
        /// ?O???b?h?????g?p?̏ꍇ?͓o?^????
        /// ?g?p????Ă???ꍇ??false??Ԃ?
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryRegisterGridInfo(GridInfo info) => _gridManager.TryRegisterGridInfo(info);

        /// <summary>
        /// ?g?p?o?^????????
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryUnregisterGridInfo(GridInfo info) => _gridManager.TryUnregisterGridInfo(info);

        /// <summary>
        /// ?w?肵???O???b?h??n?C???C?g????
        /// ???͂????X?g?ɂȂ??ꍇ?̓n?C???C?g?????
        /// </summary>
        /// <param name="index">?O???b?h?̃C???f?b?N?X?ԍ?</param>
        public void HighLightGrid(GridInfo info) => _gridManager.HighLightGrid(info);

        /// <summary>
        /// ?O???b?h???g?p?ς݂ɓo?^????Ă??邩?ǂ???
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsRegisterGridInfo(GridInfo info) => _gridManager.IsRegisterGridInfo(info);
    }
}
