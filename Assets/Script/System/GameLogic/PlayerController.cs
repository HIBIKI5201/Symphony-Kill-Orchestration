using SymphonyFrameWork.CoreSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Orchestration.System
{
    public class PlayerController : MonoBehaviour
    {

        private PlayerInput _input;

        public InputContext<Vector2> Move;
        public InputContext<float> Look;


        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            _input.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

            Move = new InputContext<Vector2>(_input.actions["Move"]);
            Look = new InputContext<float>(_input.actions["Look"]);
        }

        public class InputContext<T> where T : struct
        {
            /// <summary>
            /// イベント登録
            /// </summary>
            /// <param name="action"></param>
            public InputContext(InputAction action)
            {
                action.started += cbc => ValueInvoke(cbc, OnStarted);
                action.performed += cbc => ValueInvoke(cbc, OnPerformed);
                action.canceled += cbc => ValueInvoke(cbc, OnCanseled);

                void ValueInvoke(InputAction.CallbackContext context, Action<T> action)
                {
                    T value = context.ReadValue<T>();
                    action?.Invoke(value);
                }
            }

            /// <summary>
            /// イベントを解放
            /// </summary>
           ~InputContext()
            {
                OnStarted = null;
                OnPerformed = null;
                OnCanseled = null;
            }

            public event Action<T> OnStarted;
            public event Action<T> OnPerformed;
            public event Action<T> OnCanseled;
        }
    }
}
