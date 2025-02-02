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

        [SerializeField]
        private Transform _target;
        public Transform Target { get => _target; }

        private Vector2 _currentDirection;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent.NullCheckComponent("NavMeshAgent��������܂���"))
            {
                //�蓮�œ��������߃A�b�v�f�[�g�͂Ȃ�
                _agent.updatePosition = false;
                _agent.updateRotation = false;

                _agent.autoTraverseOffMeshLink = true;
            }

            _animator = GetComponentInChildren<Animator>();
            if (_animator.NullCheckComponent("Animator��������܂���"))
            {
                _animator.applyRootMotion = false;
            }

            if (_target.NullCheckComponent("�^�[�Q�b�g��������܂���"))
            {
                _target.parent = null;
            }
        }

        public void Move()
        {
            Vector3 localNextPos = transform.InverseTransformPoint(_agent.nextPosition);
            Vector2 targetDirection = new Vector2(localNextPos.x, localNextPos.z).normalized;

            //Lerp�Ŋ��炩�ɕω�
            _currentDirection = Vector2.Lerp(_currentDirection, targetDirection, Time.deltaTime * 5);

            _animator.SetFloat("Right", _currentDirection.x);
            _animator.SetFloat("Forward", _currentDirection.y);

            //���g�̈ʒu��Agent�ɓ���
            transform.position = _agent.nextPosition;

            Vector3 direction = _target.position - transform.position;
            direction.y = 0;  // Y�������𖳎�

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Lerp�Ŋ��炩�ɕω�
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 5);
        }

        public void SetDirection()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //�q�b�g�����ꏊ��ڕW�n�_�ɂ���
                Vector3 hitPosition = hit.point;
                _agent.SetDestination(hitPosition);
            }
        }
    }
}