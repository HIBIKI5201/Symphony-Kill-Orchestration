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
            //???͂ɓG??????ꍇ?͍U???A???Ȃ??ꍇ?͈ړ??????????
            if (_attack.SearchTarget(_data.AttackRange, _model.TargetLayer, out var enemy))
            {
                if (_attack.CanAttack(_data.AttackRatePerMinute))
                {
                    AttackProccess(enemy);
                    _lastTarget = enemy;
                }

                //?????œG?̕????Ɍ???
                return ((enemy.transform.position - transform.position).normalized, 5);
            }
            else
            {
                //???????ړ??????Ɍ???
                return (_model.Agent.velocity.normalized, 3);
            }
        }

        public override void AddDamage(AttackData data, SoldierManager target)
        {
            base.AddDamage(data, target);

            //????U???O???猂???ꂽ?ꍇ?͋ߕt??
            if (_data.AttackRange < Vector3.Distance(target.transform.position, transform.position))
            {
                GoToTarget(target);
            }
            //?U?????͈͓????Ȃ瓮???Ȃ?
            else
            {
                //?ڕW?n?_?????n?_?łȂ??ꍇ?͎??n?_?ɍX?V????
                NavMeshAgent agent = _model.Agent;
                if (agent.isActiveAndEnabled && !agent.pathPending)
                {
                    if (agent.remainingDistance > 2) //?ׂ̃O???b?h?ȏ?̋?????????ꍇ?̂?
                    {
                        GroundManager manager = ServiceLocator.GetInstance<GroundManager>();

                        //?ړ??????̃O???b?h?Ɉړ?????悤?ɃI?t?Z?b?g?𐶐?
                        Vector3 direction = agent.velocity.normalized;
                        direction *= manager.GridSize;

                        _move.SetDestination(_model.Agent, transform.position + direction);
                    }
                }
            }
        }

        /// <summary>
        /// ?Ώۂ̏ꏊ?̋߂??ɍs???i???D??T???j
        /// </summary>
        /// <param name="target"></param>
        public void GoToTarget(SoldierManager target)
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();

            Vector3 start = target.transform.position;

            // ?e???ʂ???z??i4?????j
            Vector3[] directions = new Vector3[]
            {
                Vector3.right, Vector3.left, Vector3.forward, Vector3.back
            }.Select(v => v * manager.GridSize).ToArray();

            GridInfo info = null;
            var queue = new Queue<Vector3>(); // BFS?̒T???p?L???[
            var visited = new HashSet<Vector3>(); // ?T???ς݃??X?g?i?d???h?~?j

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector3 current = queue.Dequeue(); // ?L???[?̐擪??擾

                foreach (var dir in directions)
                {
                    Vector3 nextPos = current + dir;

                    // ???łɖK?ꂽ?ꏊ?̓X?L?b?v
                    if (visited.Contains(nextPos))
                    {
                        continue;
                    }

                    visited.Add(nextPos); // ?K??ς݂Ƃ??ă}?[?N

                    // ?O???b?h?????邩?m?F
                    if (manager.GetGridByPosition(nextPos, out info) && !manager.IsRegisterGridInfo(info))
                    {
                        goto End;
                    }

                    // ???̒T?????Ƃ??Ēǉ?
                    queue.Enqueue(nextPos);
                }
            }

        End:
            SetDestination(info.transform.position);
            return; // ?ł?߂??O???b?h????????????I??
        }

        private void OnDestroy()
        {
            var system = ServiceLocator.GetInstance<IngameSystemManager>();
            //???S???ɃV?X?e???ɒʍ?
            if (system)
            {
                system.KillEnemy();
            }
        }
    }
}
