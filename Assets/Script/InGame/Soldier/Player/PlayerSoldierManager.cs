using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Orchestration.Entity
{
    public class PlayerSoldierManager : SoldierManager
    {
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

        public void Select(bool active)
        {
            _ui.Select(active);

            IconSelect(_model.MiniMapIcon, active);
        }

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

        protected override void OnDeath()
        {
            var manager = ServiceLocator.GetInstance<UnitManager>();
            manager.DeathSoldier(this);

            base.OnDeath();
        }
    }
}
