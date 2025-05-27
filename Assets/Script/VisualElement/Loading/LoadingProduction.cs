using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class LoadingProduction : SymphonyVisualElement
    {
        private VisualElement _progressBar;

        public LoadingProduction() : base("UXML/Loading/LoadingProduction") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _progressBar = container.Q<VisualElement>("ProgressBar");

            return Task.CompletedTask;
        }

        public void ProgressUpdate(float progress)
        {
            if (progress < 0 || 1 < progress)
            {
                Debug.LogWarning("???l?͈̔͊O?ł?");
                return;
            }

            _progressBar.style.width = Length.Percent(progress * 100);
        }
    }
}
