using UnityEngine;

namespace Orchestration.Entity
{
    public class MedicSkill : SkillBase
    {
        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("メディックスキル発動");
            return true;
        }
    }
}
