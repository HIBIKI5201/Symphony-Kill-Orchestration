using UnityEngine;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitSelectorSoldier : SymphonyVisualElement
    {
        private VisualElement _icon;
        private VisualElement _specialPointGuage;
        private Label _specialPointCount;

        public UnitSelectorSoldier() : base("UXML/Ingame/HUD/UnitSelector-Soldier", InitializeType.FullRangth) { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            this.style.maxWidth = Length.Percent(30);
            this.style.minWidth = Length.Percent(15);

            return Task.CompletedTask;
        }

        public void Init(Texture2D texture)
        {
            _icon.style.backgroundImage = new StyleBackground(texture);
        }

        public void UpdateSpecialPointInfo(float proportion, int count)
        {

        }

        public void Destroy()
        {
            RemoveFromHierarchy();
        }
    }
}
