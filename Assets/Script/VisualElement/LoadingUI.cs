using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class LoadingUI : SymphonyVisualElement
    {
        public LoadingUI() : base("UXML/Loading/LoadingProduction")
        {

        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            return Task.CompletedTask;
        }
    }
}
