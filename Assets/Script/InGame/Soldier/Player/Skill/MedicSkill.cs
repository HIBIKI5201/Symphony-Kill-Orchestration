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
        private float _range = 5;

        [SerializeField]
        private GameObject _particle;

        [SerializeField]
        private GameObject _rangePrefab;

        private void Start()
        {
            _rangePrefab = Instantiate(_rangePrefab, transform.position, Quaternion.identity, transform);
            _rangePrefab.SetActive(false);

            _rangePrefab.transform.localScale = new Vector3(_range, _rangePrefab.transform.localScale.y, _range);
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
            var unit = ServiceLocator.GetInstance<UnitManager>();
            if (unit)
            {
                foreach (var s in unit.UnitSoldiers)
                {
                    //”ÍˆÍ“à‚Ì–¡•û‚ÉŽg—p
                    if (Vector3.Distance(s.transform.position, transform.position) < _range)
                    {
                        float damagedHealth = s.Data.MaxHealthPoint - s.Data.HealthPoint; //í‚ç‚ê‚½‘Ì—Í—Ê
                        damagedHealth *= _healAmountPercent / 100;

                        s.AddHeal(damagedHealth);
                        Instantiate(_particle, s.transform.position, Quaternion.identity);
                    }
                }
            }

            return true;
        }
    }
}
