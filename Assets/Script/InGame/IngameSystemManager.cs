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
        public event Action OnResultEnd;

        private int _activeEnemyValue = 0;

        [SerializeField]
        private float _stageLimit = 10;
        private float _stageTimer = 0;

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

            _stageTimer = Time.time;
        }

        private void Update()
        {
            if (_stageTimer + _stageLimit < Time.time)
            {
                NextStage();
            }
        }

        public void NextStage()
        {
            _stageCounter++;
            OnStageChanged?.Invoke(_stageCounter);

            _stageTimer = Time.time;
        }

        public void KillEnemy()
        {
            _killCounter++;
            OnKillCounterChanged?.Invoke(_killCounter);

            _activeEnemyValue--;
            if (_activeEnemyValue <= 0)
            {
                NextStage();
            }
        }

        /// <summary>
        /// アクティブな敵を追加
        /// </summary>
        /// <param name="value"></param>
        public void AddAcviveEnemy(int value)
        {
            _activeEnemyValue += value;
        }

        public async void ResultOpen()
        {
            //ゲームをポーズする
            PauseManager.Pause = true;

            //リザルト開始時のイベント
            OnResultOpen?.Invoke();

            int score = _killCounter * 100 + _stageCounter * 50;

            //リザルトウィンドウの演出
            var ui = ServiceLocator.GetInstance<IngameUIManager>();
            await ui.ResultWindowStart(score, _stageCounter * 10, _killCounter);


            //リザルト演出終了時のイベント
            OnResultEnd?.Invoke();
        }
    }
}
