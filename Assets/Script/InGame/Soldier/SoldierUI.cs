using Orchestration.InGame;
using Orchestration.UI;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.Entity
{
    public class SoldierUI : MonoBehaviour
    {
        private UnitInfomationSoldier _info;
        private UnitSelectorSoldier _selector;

        private UIDocument _document;
        private VisualElement _healthBar;
        
        private void Awake()
        {
            _document = GetComponentInChildren<UIDocument>();
            if (_document.NullCheckComponent("UI Documentが見つかりません"))
            {
                _healthBar = _document.rootVisualElement.Q<VisualElement>("health-bar");
            }
        }

        public void AddInfomationForHUD(string name, Texture2D icon)
        {
            var ingameUIManager = ServiceLocator.GetInstance<IngameUIManager>();
            if (ingameUIManager)
            {
                _info = new UnitInfomationSoldier();
                _info.Init(name);
                ingameUIManager.AddSoldierInfo(_info);

                _selector = new UnitSelectorSoldier();
                _selector.Init(icon);
                ingameUIManager.AddSoldierSelector(_selector);
            }
        }

        /// <summary>
        /// ヘルスバーの位置を更新する
        /// </summary>
        /// <param name="pos"></param>
        public void HealthBarMove(Vector3 pos, Vector3 healthBarOffset)
        {
            if (_healthBar == null)
            {
                return;
            }

            //スクリーン座標系に変換
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos + healthBarOffset);

            float centerX = screenPos.x - (_healthBar.resolvedStyle.width / 2);
            float centerY = Screen.height - screenPos.y; //UITK座標系では値が高いほど下に移動する

            _healthBar.style.left = centerX;
            _healthBar.style.top = centerY;
        }

        /// <summary>
        /// ヘルスバーの量を更新する
        /// </summary>
        /// <param name="proportion"></param>
        public void HealthBarUpdate(float proportion) => _info.HealthBarUpdate(proportion);

        /// <summary>
        /// スペシャルポイントの量を更新する
        /// </summary>
        /// <param name="proportion"></param>
        /// <param name="count"></param>
        public void SpecialPointGuageUpdate(float proportion) => _selector.SpecialPointGuageUpdate(proportion);

        public void SpecialPointCountUpdate(int count) => _selector.SpecialPointCountUpdate(count);
        private void OnDestroy()
        {
            _info?.Destroy();
            _info = null;

            _selector?.Destroy();
            _selector = null;
        }
    }
}
