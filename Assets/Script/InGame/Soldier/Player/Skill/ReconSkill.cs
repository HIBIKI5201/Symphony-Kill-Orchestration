using UnityEngine;

namespace Orchestration.Entity
{
    public class ReconSkill : SkillBase
    {
        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("���R���X�L������");
            return true;
        }
    }
}
