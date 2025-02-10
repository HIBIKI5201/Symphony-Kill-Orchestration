using UnityEngine;

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
            //���͂ɓG������ꍇ�͍U���A���Ȃ��ꍇ�͈ړ�����������
            if (_attack.SearchTarget(_soldierData.AttackRange, _model.TargetLayer, out var enemy))
            {
                if (_attack.CanAttack(_soldierData.AttackRatePerMinute))
                {
                    _attack.AttackEnemy(enemy, _soldierData.Attack);
                    _model.Shoot();

                    _lastTarget = enemy;
                }

                //�����œG�̕����Ɍ���
                return ((enemy.transform.position - transform.position).normalized, 5);
            }
            else
            {
                //�������ړ������Ɍ���
                return (_model.Agent.velocity.normalized, 3);
            }
        }

    }
}
