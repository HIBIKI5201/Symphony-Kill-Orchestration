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
            //味方にバフを追加する

            Func<float, float> buff = value =>
            {
                if (value < 0)
                {
                    value = 0;
                }
                return value;
            };

            //味方全員にバフを与える
            var unit = ServiceLocator.GetInstance<UnitManager>();
            if (unit)
            {
                foreach (var s in unit.UnitSoldiers)
                {
                    s.AttackBuff(buff, true);
                    Instantiate(_particle, s.transform.position, Quaternion.identity);
                }
            }

            BuffCancel(buff, soldier);

            return true;
        }

        private async void BuffCancel(Func<float, float> buff, SoldierManager soldier)
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
                        s.AttackBuff(buff, false);
                    }
                }
            }
        }
    }
}
