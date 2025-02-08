using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitSelector : SymphonyVisualElement
    {
        public UnitSelector() : base("UXML/Ingame/HUD/UnitSelector") { }
        protected override Task Initialize_S(TemplateContainer container)
        {
            return Task.CompletedTask;
        }
    }
}
