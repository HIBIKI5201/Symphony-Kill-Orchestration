using UnityEngine;

namespace Orchestration.Entity
{
    public class SpportSkill : SkillBase
    {
        public override void SkillActive(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("サポートスキル発動");
        }
    }
}
