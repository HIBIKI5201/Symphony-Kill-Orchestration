using UnityEngine;

namespace Orchestration.Entity
{
    public class MedicSkill : SkillBase
    {
        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Debug.Log("���f�B�b�N�X�L������");
            return true;
        }
    }
}
