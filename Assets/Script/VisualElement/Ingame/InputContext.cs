using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class InputContext : SymphonyVisualElement
    {
        private VisualElement _explanation;

        public InputContext() : base("UXML/Ingame/HUD/InputContext") { }
        protected override Task Initialize_S(TemplateContainer container)
        {
            _explanation = container.Q<VisualElement>("explanation");
            return Task.CompletedTask;
        }

        public void ShowExplanation() => _explanation.style.display = DisplayStyle.Flex;
        public void HideExplanation() => _explanation.style.display = DisplayStyle.None;
    }
}
