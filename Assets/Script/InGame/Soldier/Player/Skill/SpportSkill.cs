using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using System;
using UnityEngine;

namespace Orchestration.Entity
{
    public class SpportSkill : SkillBase
    {
        [SerializeField]
        private float _duration = 6;

        [SerializeField]
        GameObject _particle;

        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            //–¡•û‘Sˆõ‚Éƒoƒt‚ð—^‚¦‚é
            var unit = ServiceLocator.GetInstance<UnitManager>();
            if (unit)
            {
                foreach (var s in unit.UnitSoldiers)
                {
                    s.SupportBuff(true);
                    Instantiate(_particle, s.transform.position, Quaternion.identity);
                }
            }

            BuffCancel();

            return true;
        }

        private async void BuffCancel()
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(_duration, destroyCancellationToken);
            }
            finally
            {
                var unit = ServiceLocator.GetInstance<UnitManager>();
                if (unit)
                {
                    foreach (var s in unit.UnitSoldiers)
                    {
                        s.SupportBuff(false);
                    }
                }
            }
        }
    }
}
