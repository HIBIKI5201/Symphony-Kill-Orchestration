using UnityEngine;

namespace Orchestration.Entity
{
    public class SpportSkill : SkillBase
    {
        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("サポートスキル発動");
            return true;
        }
    }
}
