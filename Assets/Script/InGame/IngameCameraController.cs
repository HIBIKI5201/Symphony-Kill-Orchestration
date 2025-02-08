using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Orchestration
{
    public class IngameCameraController : MonoBehaviour
    {
        private CinemachineCamera _camera;

        [SerializeField]
        private float _speed = 4;

        private Vector2 _velocity;
        private void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            PlayerController controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                //à⁄ìÆì¸óÕÇvelocityÇ…ãLò^
                controller.Move.OnPerformed += d => _velocity = d;
                controller.Move.OnCanseled +=d => _velocity = Vector2.zero;
            }
        }

        private void Update()
        {
            //ÉJÉÅÉâÇÃà⁄ìÆ
            transform.position += new Vector3(_velocity.x, 0, _velocity.y).normalized * _speed * Time.deltaTime;
        }
    }
}
