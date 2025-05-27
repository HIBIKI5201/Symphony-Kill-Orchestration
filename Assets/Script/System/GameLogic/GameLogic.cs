using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class GameLogic : MonoBehaviour
    {
        private SceneChanger _changer;
        private SystemUIManager _systemUIManager;

        public bool IsSceneLoading { get => _changer.IsLoading; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void SystemSceneLoad()
        {
            await SceneLoader.LoadScene("System");
        }

        private void Awake()
        {
            _changer = new SceneChanger(1, 1);
            _systemUIManager = GetComponentInChildren<SystemUIManager>();
        }

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        public void SceneChange(SceneEnum scene)
        {
            _changer.SceneLoad(scene);
        }

        public async Awaitable FadeIn(float time) => await _systemUIManager.FadeIn(time);
        public async Awaitable FadeOut(float time) => await _systemUIManager.FadeOut(time);
    }
}
