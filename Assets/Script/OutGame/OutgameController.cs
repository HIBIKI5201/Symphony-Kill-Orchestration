using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.OutGame
{
    public class OutgameController : MonoBehaviour
    {
        private void Start()
        {
            //�����̓��͂����������ɃQ�[�����J�n����
            var playerController = ServiceLocator.GetInstance<PlayerController>();
            if (playerController)
            {
                playerController.Active.OnStarted += StartGameFloat;
                playerController.Move.OnStarted += StartGameVector;
                playerController.Select.OnStarted += StartGameFloat;
            }

            //�Q�[���X�^�[�g�̃��\�b�h
            void StartGame()
            {
                GameLogic logic = ServiceLocator.GetInstance<GameLogic>();

                //���[�h���łȂ���΃Q�[�����X�^�[�g
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
