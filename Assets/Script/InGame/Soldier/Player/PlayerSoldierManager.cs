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

            if (_soldierData != null && _ui.NullCheckComponent($"{name}のUIが見つかりませんでした"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / _soldierData.MaxHealthPoint);

                _soldierData.OnSpecialPointChanged += value => _ui.SpecialPointCountUpdate(value);
            }
        }

        protected override void Update_S()
        {
            base.Update_S();

            //ヘルスバーの位置更新
            _ui.HealthBarMove(transform.position, _model.HealthBarOffset);

            //移動経路線を描画
            if (_model.Agent.path.corners.Length > 0)
            {
                _ui.MoveLineRender(_model.Agent, _model.MoveLineRenderer);
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            var manager = ServiceLocator.GetInstance<UnitManager>();
        }
    }
}
