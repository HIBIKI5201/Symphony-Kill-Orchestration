using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class SkillInfomation : SymphonyVisualElement
    {
        private Label _name;
        private Label _explanation;

        public SkillInfomation() : base("UXML/Ingame/HUD/SkillInfomation") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _name = container.Q<Label>("name");
            _explanation = container.Q<Label>("explanation");

            return Task.CompletedTask;
        }

        public void SetSkillInfo(string name, string explanation)
        {
            _name.text = name;
            _explanation.text = explanation;
        }
    }
}
