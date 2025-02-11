using UnityEngine;

namespace Orchestration.Entity
{
    public class ReconSkill : SkillBase
    {
        [SerializeField]
        private LayerMask _target;
        [SerializeField]
        private GameObject _particle;
        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("ƒŠƒRƒ“ƒXƒLƒ‹”­“®");

            var attackModule = GetComponent<SoldierAttack>();

            if (attackModule.SearchTarget(data.AttackRange, _target, out var s))
            {
                attackModule.AttackEnemy(s, new(data.Attack * 5, true), soldier);
                Instantiate(_particle, s.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                return true;
            }

            return false;
        }
    }
}
