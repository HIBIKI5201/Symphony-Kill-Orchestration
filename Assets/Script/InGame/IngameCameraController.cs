using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameCameraController : MonoBehaviour, PauseManager.IPausable
    {
        private CinemachineCamera _camera;
        private CinemachineBrain _brain;

        private Transform _configer;

        [SerializeField]
        private float _speed = 4;

        [Space]

        [SerializeField]
        private float _configerSpeed = 1;

        private Vector2 _velocity;

        private float _firstPosX;

        private bool _isPause = false;

        public void Pause() => _isPause = true;
        public void Resume() => _isPause = false;

        private void OnEnable()
        {
            PauseManager.IPausable.RegisterPauseManager(this);
        }

        private void OnDisable()
        {
            PauseManager.IPausable.UnregisterPauseManager(this);
        }

        private void Awake()
        {
            _camera = GetComponentInChildren<CinemachineCamera>();
            _configer = GetComponentInChildren<Collider>().transform;

            _firstPosX = _configer.transform.position.x;
        }

        private void Start()
        {
            _brain = Camera.main.GetComponent<CinemachineBrain>();


            _brain.transform.position = _camera.transform.position;

            PlayerController controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                //移動入力をvelocityに記録
                controller.Move.OnPerformed += UpdateVelocity;
                controller.Move.OnCanseled += ResetVelocity;
            }

            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += MoveConfiger;
        }

        private void OnDestroy()
        {
            PlayerController controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                //移動入力をvelocityに記録
                controller.Move.OnPerformed -= UpdateVelocity;
                controller.Move.OnCanseled -= ResetVelocity;
            }
        }

        private void UpdateVelocity(Vector2 direction) => _velocity = direction;
        private void ResetVelocity(Vector2 direction) => _velocity = Vector2.zero;

        private void Update()
        {
            if (!_isPause)
            {
                //Brainと位置を同期
                _camera.transform.position = _brain.transform.position;

                //カメラの移動
                _camera.transform.position += new Vector3(_velocity.x, 0, _velocity.y).normalized * _speed * Time.deltaTime;
            }
        }

        private async void MoveConfiger(int count)
        {
            float nextPosX = (count) * GroundManager.ChunkSize + _firstPosX;

            //次のステージ位置に移動するまで繰り返す
            while (nextPosX >= _configer.position.x)
            {
                _configer.position += new Vector3(_configerSpeed * Time.deltaTime, 0, 0);

                try
                {
                    await Awaitable.NextFrameAsync(destroyCancellationToken);
                }
                catch
                {
                    return;
                }
            }

            //移動完了したら整数値に戻す
            _configer.position = new Vector3(nextPosX, _configer.position.y, _configer.position.z);
        }
    }
}
