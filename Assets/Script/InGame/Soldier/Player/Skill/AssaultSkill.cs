using System.Linq;
using UnityEngine;

namespace Orchestration.Entity
{
    public class AssaultSkill : SkillBase
    {
        private float _buffStrength = 50;

        private float _duration = 4;

        [SerializeField]
        private LayerMask _target;

        public override void SkillActive(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, data.AttackRange, _target);

            if (colliders.Length > 0)
            {
                //周囲の敵を全て取得する
                SoldierManager[] soldiers = colliders
                    .Select(c => c.GetComponent<SoldierManager>())
                    .Where(sm => sm).ToArray();

                //バフを追加する
                int count = soldiers.Length;
                soldier.AttackBuff(Buff, true);

                var attackModule = GetComponent<SoldierAttack>();

                //兵士にダメージを与える
                foreach (var s in soldiers)
                {
                    attackModule.AttackEnemy(s, data.Attack, soldier);
                }

                //バフの効果
                float Buff(float damage)
                {
                    return damage * (1 + (_buffStrength * count / 100));
                }
            }
                Debug.Log("アサルトスキル発動");
        }


    }
}
