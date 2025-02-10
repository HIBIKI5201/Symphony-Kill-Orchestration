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
            //üˆÍ‚É“G‚ª‚¢‚éê‡‚ÍUŒ‚A‚¢‚È‚¢ê‡‚ÍˆÚ“®•ûŒü‚ğŒü‚­
            if (_attack.SearchTarget(_soldierData.AttackRange, _model.TargetLayer, out var enemy))
            {
                if (_attack.CanAttack(_soldierData.AttackRatePerMinute))
                {
                    _attack.AttackEnemy(enemy, _soldierData.Attack);
                    _model.Shoot();

                    _lastTarget = enemy;
                }

                //‚‘¬‚Å“G‚Ì•ûŒü‚ÉŒü‚­
                return ((enemy.transform.position - transform.position).normalized, 5);
            }
            else
            {
                //‚ä‚Á‚­‚èˆÚ“®•ûŒü‚ÉŒü‚­
                return (_model.Agent.velocity.normalized, 3);
            }
        }

    }
}
