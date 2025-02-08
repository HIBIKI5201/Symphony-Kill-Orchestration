using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Orchestration
{
    public class IngameCameraController : MonoBehaviour, PauseManager.IPausable
    {
        private CinemachineCamera _camera;
        private CinemachineBrain _brain;

        [SerializeField]
        private float _speed = 4;

        private Vector2 _velocity;

        private bool _isPause = false;

        public void Pause() => _isPause = true;
        public void Resume() => _isPause = false;

        private void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();

            PauseManager.IPausable.RegisterPauseManager(this);
        }

        private void Start()
        {
            PlayerController controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                //移動入力をvelocityに記録
                controller.Move.OnPerformed += d => _velocity = d;
                controller.Move.OnCanseled += d => _velocity = Vector2.zero;
            }

            _brain = Camera.main.GetComponent<CinemachineBrain>();
        }

        private void Update()
        {
            if (!_isPause)
            {
                //Brainと位置を同期
                transform.position = _brain.transform.position;

                //カメラの移動
                transform.position += new Vector3(_velocity.x, 0, _velocity.y).normalized * _speed * Time.deltaTime;
            }
        }
    }
}
