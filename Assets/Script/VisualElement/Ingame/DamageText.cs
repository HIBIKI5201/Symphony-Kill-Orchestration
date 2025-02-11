using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class DamageText : SymphonyVisualElement
    {
        private Label _text;

        public DamageText() : base("UXML/Ingame/DamageText") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _text = container.Q<Label>("Text");

            return Task.CompletedTask;
        }

        public async void Init(float damage, Vector3 position)
        {
            _text.text = damage.ToString("0.0");

            //�X�N���[�����W�n�ɕϊ�
            Vector2 screenPos = Camera.main.WorldToScreenPoint(position);

            float centerX = screenPos.x - (_text.resolvedStyle.width / 2);
            float centerY = Screen.height - screenPos.y; //UITK���W�n�ł͒l�������قǉ��Ɉړ�����

            _text.style.left = centerX;
            _text.style.top = centerY;

            //���Ԃ�҂�����ɏ���
            await Awaitable.WaitForSecondsAsync(1);

            RemoveFromHierarchy();
        }
    }
}
