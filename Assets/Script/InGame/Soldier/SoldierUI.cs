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

        private UIDocument _document;
        private VisualElement _healthBar;
        
        private void Awake()
        {
            _document = GetComponentInChildren<UIDocument>();
            if (_document.NullCheckComponent("UI Document��������܂���"))
            {
                _healthBar = _document.rootVisualElement.Q<VisualElement>("health-bar");
            }
        }

        public void Init(string name)
        {
            var ingameUIManager = ServiceLocator.GetInstance<IngameUIManager>();
            if (ingameUIManager)
            {
                _info = new UnitInfomationSoldier();
                _info.SetName(name);
                ingameUIManager.AddSoldierInfo(_info);
            }
        }

        /// <summary>
        /// �w���X�o�[�̈ʒu���X�V����
        /// </summary>
        /// <param name="pos"></param>
        public void HealthBarMove(Vector3 pos, Vector3 healthBarOffset)
        {
            if (_healthBar == null)
            {
                return;
            }

            //�X�N���[�����W�n�ɕϊ�
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos + healthBarOffset);

            float centerX = screenPos.x - (_healthBar.resolvedStyle.width / 2);
            float centerY = Screen.height - screenPos.y; //UITK���W�n�ł͒l�������قǉ��Ɉړ�����

            _healthBar.style.left = centerX;
            _healthBar.style.top = centerY;
        }

        /// <summary>
        /// �w���X�o�[�̗ʂ��X�V����
        /// </summary>
        /// <param name="proportion"></param>
        public void HealthBarUpdate(float proportion) => _info.HealthBarUpdate(proportion);

        private void OnDestroy()
        {
            _info.RemoveFromHierarchy();
            _info = null;
        }
    }
}
