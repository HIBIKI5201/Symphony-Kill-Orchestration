using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.Entity
{
    public class MedicSkill : SkillBase
    {
        [SerializeField]
        private float _healAmountPercent = 60;

        [SerializeField]
        private GameObject _particle;

        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            var unit = ServiceLocator.GetInstance<UnitManager>();
            if (unit)
            {
                foreach (var s in unit.UnitSoldiers)
                {
                    s.AddHeal(s.Data.MaxHealthPoint * _healAmountPercent / 100);
                    Instantiate(_particle, s.transform.position, Quaternion.identity);
                }
            }

            return true;
        }
    }
}
