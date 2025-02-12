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
                //�ړ����͂�velocity�ɋL�^
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
                //�ړ����͂�velocity�ɋL�^
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
                //Brain�ƈʒu�𓯊�
                _camera.transform.position = _brain.transform.position;

                //�J�����̈ړ�
                _camera.transform.position += new Vector3(_velocity.x, 0, _velocity.y).normalized * _speed * Time.deltaTime;
            }
        }

        private async void MoveConfiger(int count)
        {
            float nextPosX = (count) * GroundManager.ChunkSize + _firstPosX;

            //���̃X�e�[�W�ʒu�Ɉړ�����܂ŌJ��Ԃ�
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

            //�ړ����������琮���l�ɖ߂�
            _configer.position = new Vector3(nextPosX, _configer.position.y, _configer.position.z);
        }
    }
}
