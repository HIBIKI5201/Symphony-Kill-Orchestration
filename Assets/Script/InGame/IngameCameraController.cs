using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using System.Xml.Serialization;
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

        private bool _isPause = false;

        public void Pause() => _isPause = true;
        public void Resume() => _isPause = false;

        private void Awake()
        {
            _camera = GetComponentInChildren<CinemachineCamera>();
            _configer = GetComponentInChildren<Collider>().transform;

            PauseManager.IPausable.RegisterPauseManager(this);
        }

        private void Start()
        {
            _brain = Camera.main.GetComponent<CinemachineBrain>();

            PlayerController controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                //�ړ����͂�velocity�ɋL�^
                controller.Move.OnPerformed += d => _velocity = d;
                controller.Move.OnCanseled += d => _velocity = Vector2.zero;
            }

            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += MoveConfiger;
        }

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
            float nextPosX = (count + 1) * GroundManager.ChunkSize;

            //���̃X�e�[�W�ʒu�Ɉړ�����܂ŌJ��Ԃ�
            while (nextPosX >= _configer.position.x)
            {
                _configer.position += new Vector3(_configerSpeed * Time.deltaTime, 0, 0);

                await Awaitable.NextFrameAsync();
            }

            //�ړ����������琮���l�ɖ߂�
            _configer.position = new Vector3(nextPosX, _configer.position.y, _configer.position.z);
        }
    }
}
