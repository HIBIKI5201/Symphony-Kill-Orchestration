using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Orchestration
{
    public class OutgameController : MonoBehaviour
    {
        PlayerController _playerController;

        private void Start()
        {
            //�����̓��͂����������ɃQ�[�����J�n����
            _playerController = ServiceLocator.GetInstance<PlayerController>();
            if (_playerController)
            {
                _playerController.Active.OnStarted += StartGame;
                _playerController.Move.OnStarted += StartGame;
                _playerController.Select.OnStarted += StartGame;
            }
        }

        private void OnDestroy()
        {
            if (_playerController)
            {
                _playerController.Active.OnStarted -= StartGame;
                _playerController.Move.OnStarted -= StartGame;
                _playerController.Select.OnStarted -= StartGame;
            }
        }

        private void StartGame(float _) => StartGame();
        private void StartGame(Vector2 _) => StartGame();
        private void StartGame()
        {
            GameLogic logic = ServiceLocator.GetInstance<GameLogic>();
            logic.SceneChange(SceneEnum.InGame);

            //���͂����Z�b�g����
            _playerController.Active.Reset();
            _playerController.Move.Reset();
            _playerController.Select.Reset();
        }
    }
}
