using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class StageInfomation : SymphonyVisualElement
    {
        private Label _count;
        private Label _killCount;

        public StageInfomation() : base("UXML/Ingame/HUD/StageInfomation") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _count = container.Q<Label>("count");
            _killCount = container.Q<Label>("kill-count");

            return Task.CompletedTask;
        }

        public void CountUpdate(int count) => _count.text = count.ToString();
        public void KillCountUpdate(int count) => _killCount.text = count.ToString("000");
    }
}
