using SymphonyFrameWork.Utility;
using UnityEngine;

namespace Orchestration.Entity
{
    public class PlayerSoldierManager : SoldierManager
    {
        public override void Awake_S()
        {
            base.Awake_S();

            if (_soldierData != null && _ui.NullCheckComponent($"{name}‚ÌUI‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / _soldierData.MaxHealthPoint);

                _soldierData.OnSpecialPointChanged += value => _ui.SpecialPointCountUpdate(value);
            }
        }
    }
}
