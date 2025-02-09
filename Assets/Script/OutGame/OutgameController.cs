using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class OutgameController : MonoBehaviour
    {
        void Start()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Active.OnStarted += _ => StartGame();
                controller.Move.OnStarted += _ => StartGame();
                controller.Look.OnStarted += _ => StartGame();
                controller.Select.OnStarted += _ => StartGame();
            }
        }

        private void OnDestroy()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Active.OnStarted -= _ => StartGame();
                controller.Move.OnStarted -= _ => StartGame();
                controller.Look.OnStarted -= _ => StartGame();
                controller.Select.OnStarted -= _ => StartGame();
            }
        }

        private void StartGame()
        {
            GameLogic logic = ServiceLocator.GetInstance<GameLogic>();
            logic.SceneChange(SceneEnum.InGame);
        }
    }
}
