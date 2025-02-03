using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using System.Threading.Tasks;
using UnityEngine;

namespace Orchestration
{
    public class GameLogic : MonoBehaviour
    {
        private SceneChanger _changer;
        private SystemUIManager _systemUIManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void BeforeSceneLoad()
        {
            await SceneLoader.LoadScene("System");
        }

        private void Awake()
        {
            ServiceLocator.SetInstance(this);

            _changer = new SceneChanger();
            _systemUIManager = GetComponentInChildren<SystemUIManager>();
        }

        public void SceneChange(SceneEnum scene)
        {
            _changer.SceneLoad(scene);
        }

        public async Awaitable FadeIn(float time) => await _systemUIManager.FadeIn(time);
        public async Awaitable FadeOut(float time) => await _systemUIManager.FadeOut(time);
    }
}
