using Orchestration.UI;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.Entity
{
    public class SoldierUI : MonoBehaviour
    {
        private IngameUIManager _ingameUIManager;
        UnitInfomationSoldier _info;

        private UIDocument _document;
        private VisualElement _healthBar;
        

        [SerializeField]
        private Vector3 _healthBarOffset = Vector3.zero;
        private void Awake()
        {
            _document = GetComponentInChildren<UIDocument>();
            if (_document.NullCheckComponent("UI Documentが見つかりません"))
            {
                _healthBar = _document.rootVisualElement.Q<VisualElement>("health-bar");
            }
        }

        public void Init(string name)
        {
            _ingameUIManager = ServiceLocator.GetInstance<IngameUIManager>();
            if (_ingameUIManager)
            {
                _info = new UnitInfomationSoldier();
                _info.SetName(name);
                _ingameUIManager.AddSoldierInfo(_info);
            }
        }

        public void HealthBarMove(Vector3 pos)
        {
            if (_healthBar == null)
            {
                return;
            }

            //スクリーン座標系に変換
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos + _healthBarOffset);

            float centerX = screenPos.x - (_healthBar.resolvedStyle.width / 2);
            float centerY = Screen.height - screenPos.y; //UITK座標系では値が高いほど下に移動する

            _healthBar.style.left = centerX;
            _healthBar.style.top = centerY;
        }

        public void HealthBarUpdate(float proportion) => _info.HealthBarUpdate(proportion);
    }
}
