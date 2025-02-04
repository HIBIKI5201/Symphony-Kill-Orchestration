using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SoldierMove : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private Animator _animator;
        public Animator Animator { get => _animator; }

        private Vector2 _currentDirection;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        public void Init()
        {
            if (_agent.NullCheckComponent("NavMeshAgentが見つかりません"))
            {
                //手動で動かすためアップデートはなし
                _agent.updatePosition = false;
                _agent.updateRotation = false;

                _agent.autoTraverseOffMeshLink = true;
            }

            if (_animator.NullCheckComponent("Animatorが見つかりません"))
            {
                _animator.applyRootMotion = false;
            }
        }

        /// <summary>
        /// アニメーターに移動パラメータを渡し、座標を更新
        /// </summary>
        public void Move()
        {
            //ターゲットのベクトルを計算
            Vector3 localNextPos = transform.InverseTransformPoint(_agent.nextPosition);
            Vector2 direction = new Vector2(localNextPos.x, localNextPos.z).normalized;

            //Lerpで滑らかに変化
            _currentDirection = Vector2.Lerp(_currentDirection, direction, Time.deltaTime * 3);

            _animator.SetFloat("Right", _currentDirection.x);
            _animator.SetFloat("Forward", _currentDirection.y);

            //自身の位置をAgentに同期
            transform.position = _agent.nextPosition;
        }

        /// <summary>
        /// 引数の方向に回転させる
        /// </summary>
        /// <param name="direction"></param>
        public void Rotation(Vector3 direction)
        {
            // 進行方向がある場合のみ回転
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
        }

        /// <summary>
        /// 移動場所を取得し設定
        /// </summary>
        public void SetDirection()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetSingleton<GridManager>();

                //ヒットした場所のグリッド位置を目標地点にセット
                if (gridManager.GetGridPosition(hit.point, out Vector3 pos))
                {
                    _agent.SetDestination(pos);
                }
            }
        }
    }
}