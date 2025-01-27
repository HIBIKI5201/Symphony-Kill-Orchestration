using SymphonyFrameWork.Utility;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SoldierModel : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private Animator _animator;
        public Animator Animator { get => _animator; }

        [SerializeField]
        private Transform _target;
        public Transform Target { get => _target; }


        private float _health = 100;
        public float Health
        {
            get => _health;
            set
            { 
                _health = value;
                OnHealthChanged?.Invoke(value);
            }
        }
        public event Action<float> OnHealthChanged;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent.NullCheckComponent("NavMeshAgentが見つかりません"))
            {
                _agent.updatePosition = false;
                _agent.updateRotation = false;

                _agent.SetDestination(transform.position + new Vector3(0, 0, 10));
            }

            _animator = GetComponentInChildren<Animator>();
            if (_animator.NullCheckComponent("Animatorが見つかりません"))
            {
                _animator.applyRootMotion = false;
            }

            if (_target.NullCheckComponent("ターゲットが見つかりません"))
            {
                _target.parent = null;
            }
        }
    }
}