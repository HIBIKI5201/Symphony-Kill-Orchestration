using SymphonyFrameWork.Utility;
using UnityEngine;

namespace Orchestration.Entity
{
    public class PlayerSoldierManager : SoldierManager
    {
        public override void Awake_S()
        {
            if (_soldierData != null && _ui.NullCheckComponent($"{name}��UI��������܂���ł���"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / _soldierData.MaxHealthPoint);
                _soldierData.OnHealthChanged += OnDeath;

                _soldierData.OnSpecialPointChanged += value => _ui.SpecialPointCountUpdate(value);
            }
        }
    }
}
