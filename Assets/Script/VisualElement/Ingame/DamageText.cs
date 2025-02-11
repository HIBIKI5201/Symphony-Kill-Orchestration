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

            //スクリーン座標系に変換
            Vector2 screenPos = Camera.main.WorldToScreenPoint(position);

            float centerX = screenPos.x - (_text.resolvedStyle.width / 2);
            float centerY = Screen.height - screenPos.y; //UITK座標系では値が高いほど下に移動する

            _text.style.left = centerX;
            _text.style.top = centerY;

            //時間を待った後に消す
            await Awaitable.WaitForSecondsAsync(1);

            RemoveFromHierarchy();
        }
    }
}
