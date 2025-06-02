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
            //スクロール実行中ならキャンセルする
            if (_scrollTokenSource != null)
            {
                _scrollTokenSource.Cancel();
            }

            _name.text = name;
            _explanation.text = explanation;

            //スクロールを開始
            _scrollTokenSource = new CancellationTokenSource();
            Scroll(_scrollTokenSource.Token);
        }

        private async void Scroll(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                //位置をリセット
                _explanationScroll.scrollOffset = new Vector2(_explanationScroll.scrollOffset.x, 0);

                try
                {
                    await Awaitable.WaitForSecondsAsync(2, token);
                }
                catch { }

                //下部の隠れている部分を取得
                float targetY = _explanation.worldBound.y - _explanationScroll.contentViewport.worldBound.y + 100;

                float currentY = 0;
                //全て出すまでスクロールする
                do
                {
                    currentY = _explanationScroll.scrollOffset.y;
                    float newY = Mathf.MoveTowards(currentY, targetY, 10 * Time.deltaTime);

                    _explanationScroll.scrollOffset = new Vector2(_explanationScroll.scrollOffset.x, newY);

                    // 1フレーム待機
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
