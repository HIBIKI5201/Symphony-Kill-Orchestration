using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Orchestration.Entity
{
    public class PlayerSoldierManager : SoldierManager
    {
        public void Select(bool active)
        {
            _ui.Select(active);
        }

        public override void Awake_S()
        {
            base.Awake_S();

            if (_soldierData != null && _ui.NullCheckComponent($"{name}‚ÌUI‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / _soldierData.MaxHealthPoint);

                _soldierData.OnSpecialPointChanged += value => _ui.SpecialPointCountUpdate(value);
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            var manager = ServiceLocator.GetInstance<UnitManager>();
        }
    }
}
