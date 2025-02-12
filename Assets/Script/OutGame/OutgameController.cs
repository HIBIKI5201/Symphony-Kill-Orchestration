using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.OutGame
{
    public class OutgameController : MonoBehaviour
    {
        private PlayerController _playerController;

        private void Start()
        {
            //‰½‚©‚Ì“ü—Í‚ª‚ ‚Á‚½‚ÉƒQ[ƒ€‚ğŠJn‚·‚é
            _playerController = ServiceLocator.GetInstance<PlayerController>();
            if (_playerController)
            {
                _playerController.Active.OnStarted += StartGame;
                _playerController.Move.OnStarted += StartGame;
                _playerController.Select.OnStarted += StartGame;
            }
        }

        private void StartGame(float _) => StartGame();
        private void StartGame(Vector2 _) => StartGame();
        private void StartGame()
        {
            GameLogic logic = ServiceLocator.GetInstance<GameLogic>();
            logic.SceneChange(SceneEnum.InGame);

            if (_playerController)
            {
                _playerController.Active.OnStarted -= StartGame;
                _playerController.Move.OnStarted -= StartGame;
                _playerController.Select.OnStarted -= StartGame;
            }
        }
    }
}
