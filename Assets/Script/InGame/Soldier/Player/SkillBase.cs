using Orchestration.Entity;
using UnityEngine;

namespace Orchestration
{
    public abstract class SkillBase : MonoBehaviour
    {
        public abstract void SkillActive(PlayerSoldierManager soldier);
    }
}
