using SymphonyFrameWork.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitInfomation : SymphonyVisualElement
    {
        private VisualElement _unitInfoWindow;

        private List<UnitInfomationSoldier> _soldierList = new();

        public UnitInfomation() : base("UXML/Ingame/HUD/UnitInfomation") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _soldierList.Clear();

            _unitInfoWindow = container.Q<VisualElement>("window");
            return Task.CompletedTask;
        }

        public void AddSoldierInfo(UnitInfomationSoldier info)
        {
            if (!_soldierList.Contains(info))
            {
                _soldierList.Add(info);
                _unitInfoWindow.Add(info);
            }
        }
    }
}
