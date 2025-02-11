using System;
using System.Linq;
using UnityEngine;

namespace Orchestration.Entity
{
    public class AssaultSkill : SkillBase
    {
        private float _buffStrength = 50;

        [SerializeField]
        private float _duration = 4;

        [SerializeField]
        private LayerMask _target;

        [SerializeField]
        GameObject _particle;

        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, data.AttackRange, _target);

            if (colliders.Length > 0)
            {
                Debug.Log("アサルトスキル発動");

                //周囲の敵を全て取得する
                SoldierManager[] soldiers = colliders
                    .Select(c => c.GetComponent<SoldierManager>())
                    .Where(sm => sm).ToArray();

                //バフを追加する
                int count = soldiers.Length;
                Func<float, float> buff = Buff;

                soldier.AttackBuff(buff, true);

                var attackModule = GetComponent<SoldierAttack>();

                //兵士にダメージを与える
                foreach (var s in soldiers)
                {
                    attackModule.AttackEnemy(s, new (data.Attack), soldier);
                    Instantiate(_particle,
                        s.transform.position + Vector3.one * UnityEngine.Random.Range(0f, 0.5f),
                        Quaternion.identity);
                }

                BuffCancel(buff, soldier);

                return true;

                //バフの効果
                float Buff(float damage)
                {
                    return damage * (1 + (_buffStrength * count / 100));
                }
            }
            return false;
        }

        private async void BuffCancel(Func<float, float> buff, SoldierManager soldier)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(_duration, destroyCancellationToken);
            }
            finally
            {
                soldier.AttackBuff(buff, false);
            }
        }
    }
}
