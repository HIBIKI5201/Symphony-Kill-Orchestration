using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using System;
using System.Linq;
using UnityEngine;

namespace Orchestration.Entity
{
    public class AssaultSkill : SkillBase
    {
        [SerializeField]
        private float _buffAmountPercent = 50;

        [SerializeField]
        private float _duration = 10;

        [SerializeField]
        private LayerMask _target;

        [SerializeField]
        GameObject _particle;

        [SerializeField]
        private GameObject _rangePrefab;

        private void Start()
        {
            _rangePrefab = Instantiate(_rangePrefab, transform.position, Quaternion.identity, transform);
            _rangePrefab.SetActive(false);

            var manager = GetComponent<SoldierManager>();

            if (manager)
            {
                float range = manager.Data.AttackRange;
                _rangePrefab.transform.localScale = new Vector3(range * 2, _rangePrefab.transform.localScale.y, range * 2);
            }
        }

        public override void SkillVisible()
        {
            _rangePrefab?.SetActive(true);
        }

        public override void SkillActive(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            _rangePrefab?.SetActive(false);
            base.SkillActive(soldier, data);
        }


        protected override bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, data.AttackRange, _target);

            if (colliders.Length > 0)
            {
                Debug.Log("アサルトスキル発動");

                //周囲の敵を全て取得する
                SoldierManager[] soldiers = colliders
                    .Select(c => c.GetComponent<SoldierManager>())
                    .Where(sm => sm).ToArray();

                //味方にバフを追加する
                int count = soldiers.Length;
                Func<float, float> buff = damage => damage * (1 + (_buffAmountPercent * count / 100));

                var unit = ServiceLocator.GetInstance<UnitManager>();
                if (unit)
                {
                    foreach (var s in unit.UnitSoldiers)
                    {
                        s.AttackBuff(buff, true);
                    }
                }


                var attackModule = GetComponent<SoldierAttack>();

                //兵士にダメージを与える
                foreach (var s in soldiers)
                {
                    attackModule.AttackEnemy(s, new(data.Attack), soldier);
                    Instantiate(_particle,
                        s.transform.position + Vector3.one * UnityEngine.Random.Range(0f, 0.5f),
                        Quaternion.identity);
                }

                BuffCancel(buff, soldier);

                return true;
            }
            return false;
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
