using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.UI
{
    [UxmlElement]
    public partial class DamageText : SymphonyVisualElement
    {
        private Label _text;

        public DamageText() : base("UXML/Ingame/DamageText") { }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _text = container.Q<Label>("Text");

            return Task.CompletedTask;
        }

        public async void Init(float damage, Vector3 position, bool highLight)
        {
            position += new Vector3(0, 1.5f, 0);

            await Awaitable.NextFrameAsync();

            _text.text = Mathf.Floor(damage).ToString("0");

            if (highLight)
            {
                _text.style.color = Color.yellow;
            }

            //スクリーン座標系に変換
            Vector2 screenPos = Camera.main.WorldToScreenPoint(position);

            //UITK座標系では値が高いほど下に移動する
            Vector2 center = new(screenPos.x - (_text.resolvedStyle.width / 2), Screen.height - screenPos.y);

            //物理的に飛ぶ
            Vector2 velocity = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-1f, 0f)).normalized * 750;

            //最初にズレた場所から始まる
            for (int i = 0; i < 5; i++)
            {
                Physics(ref center, ref velocity);
            }

            TextMove(center);
            
            //時間を待った後に消す
            float timer = 0;
            while (timer < 0.5f)
            {
                TextMove(center);

                Physics(ref center, ref velocity);

                velocity *= 0.9f; //段々と減速

                if (highLight)
                {
                    _text.style.fontSize = Length.Percent(40 + timer * 20);
                }

                timer += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            RemoveFromHierarchy();
        }

        private void Physics(ref Vector2 pos, ref Vector2 velocity)
        {
            velocity.y += 1000f * Time.deltaTime; //重力

            pos.x += velocity.x * Time.deltaTime;
            pos.y += velocity.y * Time.deltaTime;
        }

        private void TextMove(Vector2 position)
        {
            _text.style.left = position.x;
            _text.style.top = position.y;
        }
    }
}
