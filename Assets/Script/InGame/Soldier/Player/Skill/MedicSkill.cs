using UnityEngine;

namespace Orchestration.Entity
{
    public class MedicSkill : SkillBase
    {
        public override void SkillActive(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("メディックスキル発動");
        }
    }
}
