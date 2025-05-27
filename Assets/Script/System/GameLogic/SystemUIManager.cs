using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.System
{
    public class SystemUIManager : MonoBehaviour
    {
        private UIDocument _document;

        private VisualElement _fade;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = _document.rootVisualElement;
            _fade = root.Q<VisualElement>("Fade");
        }

        public async Awaitable FadeIn(float time)
        {
            float alpha = 1;
            while (0 < alpha)
            {
                alpha -= (1 / time) * Time.deltaTime;
                _fade.style.opacity = alpha; //?????x??X?V
                await Awaitable.NextFrameAsync();
            }
            _fade.style.opacity = 0;
        }

        public async Awaitable FadeOut(float time)
        {
            float alpha = 0;
            while (alpha < 1)
            {
                alpha += (1 / time) * Time.deltaTime;
                _fade.style.opacity = alpha; //?????x??X?V
                await Awaitable.NextFrameAsync();
            }
            _fade.style.opacity = 1;
        }
    }
}
