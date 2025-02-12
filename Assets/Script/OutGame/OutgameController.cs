using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.OutGame
{
    public class OutgameController : MonoBehaviour
    {
        private void Start()
        {
            //何かの入力があった時にゲームを開始する
            var playerController = ServiceLocator.GetInstance<PlayerController>();
            if (playerController)
            {
                playerController.Active.OnStarted += StartGame;
                playerController.Move.OnStarted += StartGame;
                playerController.Select.OnStarted += StartGame;
            }
        }

        private void StartGame(float _) => StartGame();
        private void StartGame(Vector2 _) => StartGame();
        private void StartGame()
        {
            GameLogic logic = ServiceLocator.GetInstance<GameLogic>();

            var playerController = ServiceLocator.GetInstance<PlayerController>();

            //ロード中でなければゲームをスタート
            if (!logic.IsSceneLoading && playerController)
            {
                logic.SceneChange(SceneEnum.InGame);

                playerController.Active.OnStarted -= StartGame;
                playerController.Move.OnStarted -= StartGame;
                playerController.Select.OnStarted -= StartGame;
            }
        }
    }
}
