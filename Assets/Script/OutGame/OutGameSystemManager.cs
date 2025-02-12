using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using System;
using UnityEngine;

namespace Orchestration.OutGame
{
    public class OutGameSystemManager : MonoBehaviour
    {
        public Action OnIngameLoad;

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        private void Start()
        {
            var audio = ServiceLocator.GetInstance<AudioManager>();

            audio.BGMChanged(0, 2);
        }

        public bool InGameLoad()
        {
            GameLogic logic = ServiceLocator.GetInstance<GameLogic>();
            
            if (!logic.IsSceneLoading)
            {
                OnIngameLoad?.Invoke();

                logic.SceneChange(SceneEnum.InGame);

                return true;
            }
            return false;
        }
    }
}
