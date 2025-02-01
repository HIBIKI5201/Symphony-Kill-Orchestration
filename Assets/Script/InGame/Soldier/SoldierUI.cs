using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.Entity
{
    public class SoldierUI : MonoBehaviour
    {
        private UIDocument _document;

        private VisualElement _healthBar;
        private VisualElement _bar;
        private async void Awake()
        {
            _document = GetComponentInChildren<UIDocument>();
            if (_document.NullCheckComponent("UI Document��������܂���"))
            {
                _healthBar = _document.rootVisualElement.Q<VisualElement>("health-bar");
            }

            if (_healthBar != null)
            {
                _bar = _healthBar.Q<VisualElement>("bar");
            }
        }

        public void HealthBarMove(Vector3 pos)
        {
            if (_healthBar == null)
            {
                return;
            }

            //�X�N���[�����W�n�ɕϊ�
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

            float centerX = screenPos.x - (_healthBar.resolvedStyle.width / 2);
            float centerY = Screen.height - screenPos.y; //UITK���W�n�ł͑����قǉ��Ɉړ�����

            _healthBar.style.left = centerX;
            _healthBar.style.top = centerY;
        }

        public void HealthBarUpdate(float proportion)
        {
            _bar.style.width = Length.Percent(proportion * 100);
        }
    }
}
