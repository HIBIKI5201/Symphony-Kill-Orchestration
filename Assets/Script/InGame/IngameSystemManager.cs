using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using System;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameSystemManager : MonoBehaviour
    {
        private int _stageCounter = 0;
        public int StageCounter { get => _stageCounter; }
        public event Action<int> OnStageChanged;

        private int _killCounter = 0;
        public event Action<int> OnKillCounterChanged;

        public event Action OnResultOpen;

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
            audio.BGMChanged(1, 2);
        }

        public void NextStage()
        {
            _stageCounter++;
            OnStageChanged?.Invoke(_stageCounter);
        }

        public void KillEnemy()
        {
            _killCounter++;
            OnKillCounterChanged?.Invoke(_killCounter);
        }

        public void ResultOpen()
        {
            OnResultOpen?.Invoke();

            var logic = ServiceLocator.GetInstance<GameLogic>();
            logic.SceneChange(SceneEnum.Home);
        }
    }
}
