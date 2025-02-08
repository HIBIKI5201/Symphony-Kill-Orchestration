using SymphonyFrameWork.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class UnitSelector : SymphonyVisualElement
    {
        private VisualElement _panel;

        private List<UnitSelectorSoldier> _soldierList = new();

        public UnitSelector() : base("UXML/Ingame/HUD/UnitSelector") { }
        protected override Task Initialize_S(TemplateContainer container)
        {
            _panel = container.Q<VisualElement>("SelectPanel");

            return Task.CompletedTask;
        }

        public void AddSoldierInfo(UnitSelectorSoldier info)
        {
            if (!_soldierList.Contains(info))
            {
                _soldierList.Add(info);
                _panel.Add(info);
            }
        }
    }
}
