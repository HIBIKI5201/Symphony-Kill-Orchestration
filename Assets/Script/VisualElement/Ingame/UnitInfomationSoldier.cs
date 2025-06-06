using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitInfomationSoldier : SymphonyVisualElement
    {
        private VisualElement _info;
        private Label _name;
        private VisualElement _healthBar;

        public UnitInfomationSoldier() : base("UXML/Ingame/HUD/UnitInfomation-Soldier", InitializeType.FullRangth) { }
        protected override Task Initialize_S(TemplateContainer container)
        {
            this.style.maxHeight = Length.Percent(30);
            this.style.minHeight = Length.Percent(15);

            _info = container.Q<VisualElement>("info");
            _name = container.Q<Label>("name");
            _healthBar = container.Q<VisualElement>("health-bar");

            return Task.CompletedTask;
        }

        public void Init(string name)
        {
            _name.text = name;
        }

        public void HealthBarUpdate(float proportion)
        {
            _healthBar.style.width = Length.Percent(proportion * 100);
        }

        public void Selected(bool active)
        {
            const string selectClass = "select";

            if (active)
            {
                _info.AddToClassList(selectClass);
            }
            else
            {
                _info.RemoveFromClassList(selectClass);
            }

            foreach (var element in _info.Children())
            {
                if (active)
                {
                    element.AddToClassList(selectClass);
                }
                else
                {
                    element.RemoveFromClassList(selectClass);
                }
            }

        }

        public void Destroy()
        {
            RemoveFromHierarchy();
        }
    }
}
