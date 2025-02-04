using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitInfomation : SymphonyVisualElement
    {
        public UnitInfomation() : base("UXML/Ingame/HUD/UnitInfomation")
        {

        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            return Task.CompletedTask;
        }
    }
}
