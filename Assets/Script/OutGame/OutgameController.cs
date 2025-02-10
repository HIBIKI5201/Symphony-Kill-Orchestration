using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Orchestration
{
    public class OutgameController : MonoBehaviour
    {
        private void Start()
        {
            //âΩÇ©ÇÃì¸óÕÇ™Ç†Ç¡ÇΩéûÇ…ÉQÅ[ÉÄÇäJénÇ∑ÇÈ
            var controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Active.OnStarted += StartGame;
                controller.Move.OnStarted += StartGame;
                controller.Select.OnStarted += StartGame;
            }
        }

        private void OnDestroy()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Active.OnStarted -= StartGame;
                controller.Move.OnStarted -= StartGame;
                controller.Select.OnStarted -= StartGame;
            }
        }

        private void StartGame(float _) => StartGame();
        private void StartGame(Vector2 _) => StartGame();
        private void StartGame()
        {
            GameLogic logic = ServiceLocator.GetInstance<GameLogic>();
            logic.SceneChange(SceneEnum.InGame);
        }
    }
}
