using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class MiniMap : SymphonyVisualElement
    {
        public MiniMap() : base("UXML/Ingame/HUD/MiniMap") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            return Task.CompletedTask;
        }
    }
}
