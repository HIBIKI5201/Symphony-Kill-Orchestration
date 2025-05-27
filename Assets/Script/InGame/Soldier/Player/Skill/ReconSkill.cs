using UnityEngine;

namespace Orchestration.Entity
{
    public class ReconSkill : SkillBase
    {
        [SerializeField]
        private LayerMask _target;
        [SerializeField]
        private float _damageAmountPercent = 500;
        [SerializeField]
        private GameObject _particle;
        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("???R???X?L??????");

            var attackModule = GetComponent<SoldierAttack>();

            if (attackModule.SearchTarget(data.AttackRange, _target, out var s))
            {
                attackModule.AttackEnemy(s, new(data.Attack * _damageAmountPercent / 100, true), soldier);
                Instantiate(_particle, s.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                return true;
            }

            return false;
        }
    }
}
