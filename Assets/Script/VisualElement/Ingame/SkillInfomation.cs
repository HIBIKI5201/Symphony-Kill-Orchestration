using SymphonyFrameWork.Utility;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class SkillInfomation : SymphonyVisualElement
    {
        private Label _name;
        private Label _explanation;

        private ScrollView _explanationScroll;

        private CancellationTokenSource _scrollTokenSource;

        public SkillInfomation() : base("UXML/Ingame/HUD/SkillInfomation") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _name = container.Q<Label>("name");
            _explanation = container.Q<Label>("explanation");
            _explanationScroll = container.Q<ScrollView>();

            return Task.CompletedTask;
        }

        public void SetSkillInfo(string name, string explanation)
        {
            //?X?N???[?????s???Ȃ?L?????Z??????
            if (_scrollTokenSource != null)
            {
                _scrollTokenSource.Cancel();
            }

            _name.text = name;
            _explanation.text = explanation;

            //?X?N???[????J?n
            _scrollTokenSource = new CancellationTokenSource();
            Scroll(_scrollTokenSource.Token);
        }

        private async void Scroll(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                //?ʒu????Z?b?g
                _explanationScroll.scrollOffset = new Vector2(_explanationScroll.scrollOffset.x, 0);

                try
                {
                    await Awaitable.WaitForSecondsAsync(2, token);
                }
                catch { }

                //?????̉B??Ă??镔????擾
                float targetY = _explanation.worldBound.y - _explanationScroll.contentViewport.worldBound.y + 100;

                float currentY = 0;
                //?S?ďo???܂ŃX?N???[??????
                do
                {
                    currentY = _explanationScroll.scrollOffset.y;
                    float newY = Mathf.MoveTowards(currentY, targetY, 10 * Time.deltaTime);

                    _explanationScroll.scrollOffset = new Vector2(_explanationScroll.scrollOffset.x, newY);

                    // 1?t???[???ҋ@
                    try
                    {
                        await Awaitable.NextFrameAsync(token);
                    }
                    catch { }
                }
                while (!Mathf.Approximately(_explanationScroll.scrollOffset.y, currentY));

                try
                {
                    await Awaitable.WaitForSecondsAsync(2, token);
                }
                catch { }
            }
        }
    }
}
