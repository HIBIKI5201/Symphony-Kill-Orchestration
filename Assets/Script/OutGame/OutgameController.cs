using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.OutGame
{
    public class OutgameController : MonoBehaviour
    {
        private async void Start()
        {
            await Awaitable.WaitForSecondsAsync(1.5f);

            //?????̓??͂??????????ɃQ?[????J?n????
            var playerController = ServiceLocator.GetInstance<PlayerController>();
            if (playerController)
            {
                playerController.Active.OnStarted += StartGameFloat;
                playerController.Move.OnStarted += StartGameVector;
                playerController.Select.OnStarted += StartGameFloat;
            }

            //?Q?[???X?^?[?g?̃??\?b?h
            void StartGame()
            {
                var system = ServiceLocator.GetInstance<OutGameSystemManager>();

                //???[?h???????????瑀?????
                if (system.InGameLoad() && playerController)
                {
                    playerController.Active.OnStarted -= StartGameFloat;
                    playerController.Move.OnStarted -= StartGameVector;
                    playerController.Select.OnStarted -= StartGameFloat;
                }
            }
            void StartGameFloat(float _) => StartGame();
            void StartGameVector(Vector2 _) => StartGame();
        }
    }
}
