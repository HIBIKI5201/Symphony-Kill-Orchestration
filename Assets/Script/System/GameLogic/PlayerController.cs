using SymphonyFrameWork.CoreSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Orchestration.System
{
    public class PlayerController : MonoBehaviour
    {

        private PlayerInput _playerInput;

        private InputContext<Vector2> _move;
        public InputContext<Vector2> Move => _move;

        private InputContext<float> _active;
        public InputContext<float> Active => _active;

        private InputContext<float> _select;
        public InputContext<float> Select => _select;

        private InputContext<float> _skill;
        public InputContext<float> Skill => _skill;

        private InputContext<float> _zoom;
        public InputContext<float> Zoom => _zoom;

        private InputContext<float> _input;
        public InputContext<float> Input => _input;

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
            _playerInput = GetComponent<PlayerInput>();

            if (_playerInput)
            {
                _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

                _move = new InputContext<Vector2>(_playerInput.actions["Move"]);
                _active = new InputContext<float>(_playerInput.actions["Active"]);
                _select = new InputContext<float>(_playerInput.actions["Select"]);
                _skill = new InputContext<float>(_playerInput.actions["Skill"]);
                _zoom = new InputContext<float>(_playerInput.actions["Zoom"]);
                _input = new InputContext<float>(_playerInput.actions["InputContext"]);
            }
        }

        /// <summary>
        /// ?C?x???g??R???e?i???
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class InputContext<T> where T : struct
        {
            /// <summary>
            /// ?C?x???g?o?^
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
            /// ?C?x???g?????
            /// </summary>
           ~InputContext()
            {
                Reset();
            }

            /// <summary>
            /// ?C?x???g?????Z?b?g????
            /// </summary>
            public void Reset()
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
