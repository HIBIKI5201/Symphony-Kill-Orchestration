using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class GameLogic : MonoBehaviour
    {
        private SceneChanger _changer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void BeforeSceneLoad()
        {
            await SceneLoader.LoadScene("System");
        }

        private void Awake()
        {
            ServiceLocator.SetSinglton(this);

            _changer = new SceneChanger();
        }

        public void SceneChange(SceneEnum scene)
        {
            _changer.SceneLoad(scene);
        }
    }
}
