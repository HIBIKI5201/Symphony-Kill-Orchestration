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
        public override void Awake_S()
        {
            base.Awake_S();

            if (_data != null && _ui.NullCheckComponent($"{name}のUIが見つかりませんでした"))
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

            //移動経路線を描画
            if (_model.Agent.path.corners.Length > 0)
            {
                _ui.MoveLineRender(_model.Agent, _model.MoveLineRenderer);
            }

            if (_data.SpecialPoint < _data.MaxSpecialPoint)
            {
                //毎秒スキルをチャージ
                _data.SpecialPointProportion += Time.deltaTime / 2;
            }
        }

        /// <summary>
        /// 選択された時の演出
        /// </summary>
        /// <param name="active"></param>
        public void Select(bool active)
        {
            _ui.Select(active);

            IconSelect(_model.MiniMapIcon, active);

            //スキルの情報を書き込む
            var ui = ServiceLocator.GetInstance<IngameUIManager>();
            ui.SetSkillInfo(_data.SkillName, _data.SkillExplanation);
        }

        /// <summary>
        /// ミニマップのアイコンを更新
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

        /// <summary>
        /// スキルを起動する
        /// </summary>
        public void SkillActive()
        {
            if (_skillBase)
            {
                if (_data.SpecialCost <= _data.SpecialPoint)
                {
                    _skillBase.SkillActive(this, _data);
                }
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
