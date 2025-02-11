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

        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, data.AttackRange, _target);

            if (colliders.Length > 0)
            {
                Debug.Log("�A�T���g�X�L������");

                //���͂̓G��S�Ď擾����
                SoldierManager[] soldiers = colliders
                    .Select(c => c.GetComponent<SoldierManager>())
                    .Where(sm => sm).ToArray();

                //�o�t��ǉ�����
                int count = soldiers.Length;
                soldier.AttackBuff(Buff, true);

                var attackModule = GetComponent<SoldierAttack>();

                //���m�Ƀ_���[�W��^����
                foreach (var s in soldiers)
                {
                    attackModule.AttackEnemy(s, data.Attack, soldier);
                }

                return true;

                //�o�t�̌���
                float Buff(float damage)
                {
                    return damage * (1 + (_buffStrength * count / 100));
                }
            }
            return false;
        }


    }
}
