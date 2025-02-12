using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameSystemManager : MonoBehaviour, PauseManager.IPausable
    {
        private int _stageCounter = 0;
        public int StageCounter { get => _stageCounter; }
        public event Action<int> OnStageChanged;

        private int _killCounter = 0;
        public event Action<int> OnKillCounterChanged;

        public event Action OnResultOpen;
        public event Action OnResultEnd;

        private int _activeEnemyValue = 0;
        private object _enemyValueLock = new object();

        [SerializeField]
        private float _stageLimit = 10;
        private float _stageTimer = 0;

        private bool _isPause;

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
            PauseManager.IPausable.RegisterPauseManager(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
            PauseManager.IPausable.UnregisterPauseManager(this);
        }

        private void Start()
        {
            var audio = ServiceLocator.GetInstance<AudioManager>();
            audio.BGMChanged(1, 2);

            _stageTimer = Time.time;
        }

        private void Update()
        {
            if (_isPause)
            {
                _stageTimer += Time.deltaTime;
            }

            if (_stageTimer + _stageLimit < Time.time)
            {
                NextStage();
            }
        }

        public async void NextStage()
        {
            await Task.Yield();

            if (destroyCancellationToken.IsCancellationRequested)
            {
                return;
            }

            _stageCounter++;
            OnStageChanged?.Invoke(_stageCounter);

            _stageTimer = Time.time;
        }

        public void KillEnemy()
        {
            _killCounter++;
            OnKillCounterChanged?.Invoke(_killCounter);

            ChangeActiveEnemy(-1, ActiveEnemyUpdateMode.Remove);
        }

        /// <summary>
        /// �A�N�e�B�u�ȓG��ǉ�
        /// </summary>
        /// <param name="value"></param>
        public void AddAcviveEnemy(int value) => ChangeActiveEnemy(value, ActiveEnemyUpdateMode.Add);

        private void ChangeActiveEnemy(int value, ActiveEnemyUpdateMode mode)
        {
            lock (_enemyValueLock)
            {
                _activeEnemyValue += value;

                if (mode == ActiveEnemyUpdateMode.Remove && _activeEnemyValue <= 0)
                {
                    NextStage();
                }
            }
        }

        public async void ResultOpen()
        {
            //�Q�[�����|�[�Y����
            PauseManager.Pause = true;

            //���U���g�J�n���̃C�x���g
            OnResultOpen?.Invoke();

            int score = _killCounter * 100 + _stageCounter * 50;

            //���U���g�E�B���h�E�̉��o
            var ui = ServiceLocator.GetInstance<IngameUIManager>();
            await ui.ResultWindowStart(score, _stageCounter * 10, _killCounter);


            //���U���g���o�I�����̃C�x���g
            OnResultEnd?.Invoke();
        }

        public void Pause()
        {
            _isPause = true;
        }

        public void Resume()
        {
            _isPause = false;
        }

        private enum ActiveEnemyUpdateMode { Add, Remove }
    }
}
