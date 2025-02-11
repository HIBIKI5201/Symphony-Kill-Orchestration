using UnityEngine;

namespace Orchestration.Entity
{
    public abstract class SkillBase : MonoBehaviour
    {
        public void SkillActive(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            if (SkillProccess(soldier, data))
            {
                data.SpecialPoint -= data.SpecialCost;
            }
        }

        /// <summary>
        /// スキルの処理を行う
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="data"></param>
        /// <returns>処理が成功したかどうか</returns>
        protected abstract bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data);
    }
}
