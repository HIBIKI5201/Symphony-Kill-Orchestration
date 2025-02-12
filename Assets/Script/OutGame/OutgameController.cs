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
                playerController.Active.OnStarted += StartGameFloat;
                playerController.Move.OnStarted += StartGameVector;
                playerController.Select.OnStarted += StartGameFloat;
            }

            //ゲームスタートのメソッド
            void StartGame()
            {
                GameLogic logic = ServiceLocator.GetInstance<GameLogic>();

                //ロード中でなければゲームをスタート
                if (!logic.IsSceneLoading && playerController)
                {
                    logic.SceneChange(SceneEnum.InGame);

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
