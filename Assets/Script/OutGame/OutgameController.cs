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

            //何かの入力があった時にゲームを開始する
            var playerController = ServiceLocator.GetInstance<PlayerController>();
            if (playerController)
            {
                playerController.Active.OnStarted += StartGameFloat;
                playerController.Move.OnStarted += StartGameVector;
                playerController.Select.OnStarted += StartGameFloat;
            }

            //ゲームスタートのメソッド
            void StartGame()
            {
                var system = ServiceLocator.GetInstance<OutGameSystemManager>();

                //ロードが成功したら操作を解除
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
