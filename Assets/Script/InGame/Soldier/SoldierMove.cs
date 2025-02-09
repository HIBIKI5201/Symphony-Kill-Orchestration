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
        private Vector2 _currentDirection = Vector2.zero;

        private GridInfo _currentGridInfo;

        public async void MoveGridPosition(NavMeshAgent agent)
        {
            GridManager manager = ServiceLocator.GetInstance<GridManager>();

            //初期化が終わるまで待機
            await SymphonyTask.WaitUntil(() => manager.IsInitializeDone, destroyCancellationToken);

            agent.enabled = true;

            //自分の場所を一番近いグリッドまで移動
            if (manager.GetGridPosition(transform.position, out GridInfo info))
            {
                if (manager.TryRegisterGridInfo(info))
                {
                    transform.position = info.transform.position;
                    agent.nextPosition = info.transform.position;

                    _currentGridInfo = info;
                }
            }
        }

        /// <summary>
        /// アニメーターに移動パラメータを渡し、座標を更新
        /// </summary>
        public void Move(NavMeshAgent agent, Animator animator)
        {
            //ターゲットのベクトルを計算
            Vector3 localNextPos = transform.InverseTransformPoint(agent.nextPosition);
            Vector2 direction = new Vector2(localNextPos.x, localNextPos.z).normalized;

            //Lerpで滑らかに変化
            _currentDirection = Vector2.Lerp(_currentDirection, direction, Time.deltaTime * 3);

            animator.SetFloat("Right", _currentDirection.x);
            animator.SetFloat("Forward", _currentDirection.y);

            //自身の位置をAgentに同期
            transform.position = agent.nextPosition;
        }

        /// <summary>
        /// 引数の方向に回転させる
        /// </summary>
        /// <param name="direction">向かせる方向</param>
        /// <param name="time">向くまでに掛かる平均的な時間</param>
        public void Rotation(Vector3 direction, float time = 3)
        {
            // 進行方向がある場合のみ回転
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * time);
            }
        }

        /// <summary>
        /// 移動場所を取得し設定
        /// </summary>
        public void SetDirection(NavMeshAgent agent)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetInstance<GridManager>();

                //ヒットした場所のグリッド位置が未使用なら目的地にセット
                if (gridManager.GetGridPosition(hit.point, out GridInfo info) && gridManager.TryRegisterGridInfo(info))
                {
                    agent.SetDestination(info.transform.position);

                    //前のグリッドの使用登録を解除
                    gridManager.TryUnregisterGridInfo(_currentGridInfo);
                    _currentGridInfo = info;
                }
            }
        }

        public void OnPauseMove(NavMeshAgent agent)
        {
            agent.nextPosition = transform.position;
        }
    }
}