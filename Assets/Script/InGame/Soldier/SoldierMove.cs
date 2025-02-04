using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SoldierMove : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private Animator _animator;
        public Animator Animator { get => _animator; }

        private Vector2 _currentDirection;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        public void Init()
        {
            if (_agent.NullCheckComponent("NavMeshAgent��������܂���"))
            {
                //�蓮�œ��������߃A�b�v�f�[�g�͂Ȃ�
                _agent.updatePosition = false;
                _agent.updateRotation = false;

                _agent.autoTraverseOffMeshLink = true;
            }

            if (_animator.NullCheckComponent("Animator��������܂���"))
            {
                _animator.applyRootMotion = false;
            }
        }

        /// <summary>
        /// �A�j���[�^�[�Ɉړ��p�����[�^��n���A���W���X�V
        /// </summary>
        public void Move()
        {
            //�^�[�Q�b�g�̃x�N�g�����v�Z
            Vector3 localNextPos = transform.InverseTransformPoint(_agent.nextPosition);
            Vector2 direction = new Vector2(localNextPos.x, localNextPos.z).normalized;

            //Lerp�Ŋ��炩�ɕω�
            _currentDirection = Vector2.Lerp(_currentDirection, direction, Time.deltaTime * 3);

            _animator.SetFloat("Right", _currentDirection.x);
            _animator.SetFloat("Forward", _currentDirection.y);

            //���g�̈ʒu��Agent�ɓ���
            transform.position = _agent.nextPosition;
        }

        /// <summary>
        /// �����̕����ɉ�]������
        /// </summary>
        /// <param name="direction"></param>
        public void Rotation(Vector3 direction)
        {
            // �i�s����������ꍇ�̂݉�]
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
        }

        /// <summary>
        /// �ړ��ꏊ���擾���ݒ�
        /// </summary>
        public void SetDirection()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetSingleton<GridManager>();

                //�q�b�g�����ꏊ�̃O���b�h�ʒu��ڕW�n�_�ɃZ�b�g
                if (gridManager.GetGridPosition(hit.point, out Vector3 pos))
                {
                    _agent.SetDestination(pos);
                }
            }
        }
    }
}