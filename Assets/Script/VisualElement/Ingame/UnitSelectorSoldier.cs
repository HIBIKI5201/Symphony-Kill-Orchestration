using UnityEngine;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitSelectorSoldier : SymphonyVisualElement
    {
        public UnitSelectorSoldier() : base("UXML/Ingame/HUD/UnitSelector-Soldier", InitializeType.FullRangth) { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            this.style.maxWidth = Length.Percent(30);
            this.style.minWidth = Length.Percent(15);

            return Task.CompletedTask;
        }
    }
}
