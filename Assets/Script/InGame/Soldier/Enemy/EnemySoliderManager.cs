using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    public class EnemySoliderManager : SoldierManager
    {
        private SoldierManager _lastTarget = null;

        protected override void Start_S()
        {
            _model.Init();

            _move.MoveGridPosition(_model.Agent);
        }

        protected override (Vector3, float) Attack()
        {
            //周囲に敵がいる場合は攻撃、いない場合は移動方向を向く
            if (_attack.SearchTarget(_soldierData.AttackRange, _model.TargetLayer, out var enemy))
            {
                if (_attack.CanAttack(_soldierData.AttackRatePerMinute))
                {
                    _attack.AttackEnemy(enemy, _soldierData.Attack, this);
                    _model.Shoot();

                    _lastTarget = enemy;
                }

                //高速で敵の方向に向く
                return ((enemy.transform.position - transform.position).normalized, 5);
            }
            else
            {
                //ゆっくり移動方向に向く
                return (_model.Agent.velocity.normalized, 3);
            }
        }

        public override void AddDamage(float damage, SoldierManager target)
        {
            base.AddDamage(damage, target);

            //もし攻撃外から撃たれた場合は近付く
            if (_soldierData.AttackRange < Vector3.Distance(target.transform.position, transform.position))
            {
                GoToTarget(target);
            }
            //攻撃が範囲内からなら動かない
            else
            {
                //目標地点が自地点でない場合は自地点に更新する
                NavMeshAgent agent = _model.Agent;
                if (agent.isActiveAndEnabled && !agent.pathPending)
                {
                    if (agent.remainingDistance > 2) //隣のグリッド以上の距離がある場合のみ
                    {
                        GroundManager manager = ServiceLocator.GetInstance<GroundManager>();

                        //移動方向のグリッドに移動するようにオフセットを生成
                        Vector3 direction = agent.velocity.normalized;
                        direction *= manager.GridSize;

                        _move.SetDirection(_model.Agent, transform.position + direction);
                    }
                }
            }
        }

        /// <summary>
        /// 対象の場所の近くに行く（幅優先探索）
        /// </summary>
        /// <param name="target"></param>
        public void GoToTarget(SoldierManager target)
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();

            Vector3 start = target.transform.position;

            // 各方位を持つ配列（4方向）
            Vector3[] directions = new Vector3[]
            {
                Vector3.right, Vector3.left, Vector3.forward, Vector3.back
            }.Select(v => v * manager.GridSize).ToArray();

            GridInfo info = null;
            var queue = new Queue<Vector3>(); // BFSの探索用キュー
            var visited = new HashSet<Vector3>(); // 探索済みリスト（重複防止）

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector3 current = queue.Dequeue(); // キューの先頭を取得

                foreach (var dir in directions)
                {
                    Vector3 nextPos = current + dir;

                    // すでに訪れた場所はスキップ
                    if (visited.Contains(nextPos))
                    {
                        continue;
                    }

                    visited.Add(nextPos); // 訪問済みとしてマーク

                    // グリッドがあるか確認
                    if (manager.GetGridByPosition(nextPos, out info) && !manager.IsRegisterGridInfo(info))
                    {
                        goto End;
                    }

                    // 次の探索候補として追加
                    queue.Enqueue(nextPos);
                }
            }

        End:
            SetDirection(info.transform.position);
            return; // 最も近いグリッドが見つかったら終了
        }
    }
}
