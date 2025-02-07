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
        private Vector2 _currentDirection = Vector2.zero;

        /// <summary>
        /// �A�j���[�^�[�Ɉړ��p�����[�^��n���A���W���X�V
        /// </summary>
        public void Move(NavMeshAgent agent, Animator animator)
        {
            //�^�[�Q�b�g�̃x�N�g�����v�Z
            Vector3 localNextPos = transform.InverseTransformPoint(agent.nextPosition);
            Vector2 direction = new Vector2(localNextPos.x, localNextPos.z).normalized;

            //Lerp�Ŋ��炩�ɕω�
            _currentDirection = Vector2.Lerp(_currentDirection, direction, Time.deltaTime * 3);

            animator.SetFloat("Right", _currentDirection.x);
            animator.SetFloat("Forward", _currentDirection.y);

            //���g�̈ʒu��Agent�ɓ���
            transform.position = agent.nextPosition;
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
        public void SetDirection(NavMeshAgent agent)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetInstance<GridManager>();

                //�q�b�g�����ꏊ�̃O���b�h�ʒu��ڕW�n�_�ɃZ�b�g
                if (gridManager.GetGridPosition(hit.point, out Vector3 pos))
                {
                    agent.SetDestination(pos);
                }
            }
        }
    }
}