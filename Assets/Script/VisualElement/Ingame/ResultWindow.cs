using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class ResultWindow : SymphonyVisualElement
    {
        private VisualElement _score;

        private Label _stageCounter;
        private Label _killCounter;
        private Label _scoreCounter;

        private VisualElement _buttons;

        public ResultWindow() : base("UXML/Ingame/ResultWindow") { }
        protected override Task Initialize_S(TemplateContainer container)
        {
            _score = container.Q<VisualElement>("score");

            _stageCounter = container.Q<Label>("stage-counter");
            _killCounter = container.Q<Label>("kill-counter");
            _scoreCounter = container.Q<Label>("score-counter");

            _buttons = container.Q<VisualElement>("buttons");

            _score.style.visibility = Visibility.Hidden;
            _buttons.style.visibility = Visibility.Hidden;

            style.visibility = Visibility.Hidden;

            return Task.CompletedTask;
        }

        public async Task ResultWindowStart(int score, int stage, int kill)
        {
            //?i?X?ƕ\??????鉉?o
            style.visibility = Visibility.Visible;

            await Awaitable.WaitForSecondsAsync(0.5f);

            _score.style.visibility = Visibility.Visible;

            await Awaitable.WaitForSecondsAsync(0.5f);

            //?X?e?[?W?J?E???g??J?E???g?A?b?v
            await CountUp(_stageCounter, stage, 1, "m");

            await Awaitable.WaitForSecondsAsync(0.25f);

            //?L???J?E???g??J?E???g?A?b?v
            await CountUp(_killCounter, kill, 1);


            await Awaitable.WaitForSecondsAsync(0.25f);

            //?X?R?A??J?E???g?A?b?v
            await CountUp(_scoreCounter, score, 1);

            await Awaitable.WaitForSecondsAsync(0.5f);

            _buttons.style.visibility = Visibility.Visible;
        }

        private async Task CountUp(Label label, int count, float duration, string unit = "")
        {
            float timer = Time.time;

            while (Time.time < timer + duration)
            {
                float proportion = (Time.time - timer) / duration;

                label.text = (count * proportion).ToString("0") + unit;

                await Awaitable.NextFrameAsync();
            }

            label.text = count + unit;
        }
    }
}
