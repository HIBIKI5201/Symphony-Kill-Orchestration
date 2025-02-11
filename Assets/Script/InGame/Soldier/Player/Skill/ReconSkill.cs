using UnityEngine;

namespace Orchestration.Entity
{
    public class ReconSkill : SkillBase
    {
        [SerializeField]
        private LayerMask _target;
        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("ƒŠƒRƒ“ƒXƒLƒ‹”­“®");

            var attackModule = GetComponent<SoldierAttack>();

            if (attackModule.SearchTarget(data.AttackRange, _target, out var s))
            {
                attackModule.AttackEnemy(s, data.Attack * 5, soldier);
                return true;
            }

            return false;
        }
    }
}
