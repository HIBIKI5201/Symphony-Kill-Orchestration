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
        /// ƒXƒLƒ‹‚Ìˆ—‚ğs‚¤
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="data"></param>
        /// <returns>ˆ—‚ª¬Œ÷‚µ‚½‚©‚Ç‚¤‚©</returns>
        protected abstract bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data);
    }
}
