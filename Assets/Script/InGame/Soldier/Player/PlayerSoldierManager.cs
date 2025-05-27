using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;

namespace Orchestration.Entity
{
    public class PlayerSoldierManager : SoldierManager
    {
        public Vector3 AgentDestination { get => _model.Agent.destination; }

        private SkillBase _skillBase;

        private bool _isSupportBuff; //??Uboolen?ɂ??đΏ?
        public override void Awake_S()
        {
            base.Awake_S();

            if (_data != null && _ui.NullCheckComponent($"{name}??UI????????܂???ł???"))
            {
                _data.OnHealthChanged += value =>
                    _ui.HealthBarUpdate(value / _data.MaxHealthPoint);

                _data.OnSpecialPointProportionChanged += _ui.SpecialPointGuageUpdate;
                _data.OnSpecialPointChanged += _ui.SpecialPointCountUpdate;
            }

            _skillBase = GetComponent<SkillBase>();
        }

        protected override void Update_S()
        {
            base.Update_S();

            //?ړ??o?H????`??
            if (_model.Agent.path.corners.Length > 0)
            {
                _ui.MoveLineRender(_model.Agent, _model.MoveLineRenderer);
            }

            if (_data.SpecialPoint < _data.MaxSpecialPoint)
            {
                //???b?X?L???|?C???g??`???[?W
                _data.SpecialPointProportion += Time.deltaTime / 4;
            }
        }

        public override void AddDamage(AttackData data, SoldierManager target)
        {
            if (_isSupportBuff)
            {
                data.Damage = 0;
            }
            base.AddDamage(data, target);
        }

        public void SupportBuff(bool active)
        {
            _isSupportBuff = active;
        }


        /// <summary>
        /// ?I????ꂽ???̉??o
        /// </summary>
        /// <param name="active"></param>
        public void Select(bool active)
        {
            _ui.Select(active);

            IconSelect(_model.MiniMapIcon, active);

            //?X?L???̏??????????
            var ui = ServiceLocator.GetInstance<IngameUIManager>();
            ui.SetSkillInfo(_data.SkillName, _data.SkillExplanation);
        }

        /// <summary>
        /// ?~?j?}?b?v?̃A?C?R????X?V
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="active"></param>
        private async void IconSelect(MeshRenderer renderer, bool active)
        {
            await Awaitable.NextFrameAsync();

            if (renderer)
            {
                if (active)
                {
                    renderer.material = _model.SelectedIconMaterial;
                }
                else
                {
                    renderer.material = _model.IconMaterial;
                }
            }
        }

        public void SkillVisible()
        {
            if (_skillBase)
            {
                _skillBase.SkillVisible();
            }
        }

        /// <summary>
        /// ?X?L????N??????
        /// </summary>
        public void SkillActive()
        {
            if (_skillBase)
            {
                _skillBase.SkillActive(this, _data);
            }
        }


        protected override void OnDeath()
        {
            var manager = ServiceLocator.GetInstance<UnitManager>();
            manager.DeathSoldier(this);

            base.OnDeath();
        }
    }
}
