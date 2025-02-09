using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class HomeWindow : SymphonyVisualElement
    {
        public HomeWindow() : base("UXML/OutGame/HomeWindow") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            return Task.CompletedTask;
        }
    }
}
