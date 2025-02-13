using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitSelectorSoldier : SymphonyVisualElement
    {
        private VisualElement _selecter;
        private VisualElement _icon;
        private VisualElement _specialPointGuage;
        private Label _specialPointCount;

        public UnitSelectorSoldier() : base("UXML/Ingame/HUD/UnitSelector-Soldier", InitializeType.FullRangth) { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            this.style.maxWidth = Length.Percent(30);
            this.style.minWidth = Length.Percent(15);

            _icon = container.Q<VisualElement>("icon");

            _specialPointGuage = container.Q<VisualElement>("guage");
            _specialPointCount = container.Q<Label>("count");

            _selecter = container.Q<VisualElement>("selecter");

            return Task.CompletedTask;
        }

        public void Init(Sprite texture)
        {
            _icon.style.backgroundImage = new StyleBackground(texture);
        }

        public void Selected(bool active)
        {
            const string selectClass = "select";

            if (active)
            {
                _selecter.AddToClassList(selectClass);
            }
            else
            {
                _selecter.RemoveFromClassList(selectClass);
            }
        }

        public void SpecialPointGuageUpdate(float proportion) =>
            _specialPointGuage.style.width = Length.Percent(proportion * 100);

        public void SpecialPointCountUpdate(int count) =>
            _specialPointCount.text = count.ToString();

        public void Destroy()
        {
            RemoveFromHierarchy();
        }
    }
}
