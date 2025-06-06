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
        public InputContext<float> Active;
        public InputContext<float> Select;
        public InputContext<float> Skill;
        public InputContext<float> Zoom;

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

            if (_input)
            {
                _input.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

                Move = new InputContext<Vector2>(_input.actions["Move"]);
                Active = new InputContext<float>(_input.actions["Active"]);
                Select = new InputContext<float>(_input.actions["Select"]);
                Skill = new InputContext<float>(_input.actions["Skill"]);
                Zoom = new InputContext<float>(_input.actions["Zoom"]);
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
