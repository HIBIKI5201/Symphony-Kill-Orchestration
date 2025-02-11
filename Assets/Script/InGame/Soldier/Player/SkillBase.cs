using Orchestration.Entity;
using UnityEngine;

namespace Orchestration.Entity
{
    public abstract class SkillBase : MonoBehaviour
    {
        public abstract void SkillActive(PlayerSoldierManager soldier, SoldierData_SO data);
    }
}
