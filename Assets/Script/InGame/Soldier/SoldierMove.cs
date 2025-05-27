using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    public class SoldierMove : MonoBehaviour
    {
        private Vector2 _currentDirection = Vector2.zero;

        private GridInfo _currentGridInfo;

        public async void MoveGridPosition(NavMeshAgent agent)
        {
            GroundManager manager = ServiceLocator.GetInstance<GroundManager>();

            try
            {
                //?????????I???܂őҋ@
                await SymphonyTask.WaitUntil(() => manager.GridInitializeDone, destroyCancellationToken);
            }
            catch { }

            agent.enabled = true;

            //?????̏ꏊ???ԋ߂??O???b?h?܂ňړ?
            if (manager.GetGridByPosition(transform.position, out GridInfo info))
            {
                if (manager.TryRegisterGridInfo(info))
                {
                    transform.position = info.transform.position;
                    agent.nextPosition = info.transform.position;

                    _currentGridInfo = info;
                }
            }
        }

        /// <summary>
        /// ?A?j???[?^?[?Ɉړ??p?????[?^??n???A???W??X?V
        /// </summary>
        public void Move(NavMeshAgent agent, Animator animator, AudioSource foodStepAudio)
        {
            //?^?[?Q?b?g?̃x?N?g????v?Z
            Vector3 localNextPos = transform.InverseTransformPoint(agent.nextPosition);
            Vector2 direction = new Vector2(localNextPos.x, localNextPos.z).normalized;

            //Lerp?Ŋ??炩?ɕω?
            _currentDirection = Vector2.Lerp(_currentDirection, direction, Time.deltaTime * 3);

            animator.SetFloat("Right", _currentDirection.x);
            animator.SetFloat("Forward", _currentDirection.y);

            //???g?̈ʒu??Agent?ɓ???
            transform.position = agent.nextPosition;

            //????T?E???h??Đ?
            if (0 < localNextPos.magnitude)
            {
                if (!foodStepAudio.isPlaying)
                {
                    foodStepAudio.Play();
                }
            }
            else
            {
                if (foodStepAudio.isPlaying)
                {
                    foodStepAudio.Stop();
                }
            }
        }

        /// <summary>
        /// ?????̕????ɉ?]??????
        /// </summary>
        /// <param name="direction">???????????</param>
        /// <param name="time">?????܂łɊ|???镽?ϓI?Ȏ???</param>
        public void Rotation(Vector3 direction, float time = 3)
        {
            // ?i?s??????????ꍇ?̂݉?]
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * time);
            }
        }

        /// <summary>
        /// ?ړ??ꏊ??擾???ݒ?
        /// </summary>
        public void SetDestination(NavMeshAgent agent, Vector3 point)
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();

            //?q?b?g?????ꏊ?̃O???b?h?ʒu?????g?p?Ȃ?ړI?n?ɃZ?b?g
            if (manager.GetGridByPosition(point, out GridInfo info) && manager.TryRegisterGridInfo(info))
            {
                if (agent.isActiveAndEnabled)
                {
                    agent.SetDestination(info.transform.position);

                    //?O?̃O???b?h?̎g?p?o?^????
                    manager.TryUnregisterGridInfo(_currentGridInfo);
                    _currentGridInfo = info;
                }
            }
        }

        public void OnPauseMove(NavMeshAgent agent)
        {
            agent.nextPosition = transform.position;
        }

        private void OnDestroy()
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();

            //???????????O???b?h?̎g?p?o?^????
            if (manager)
            {
                manager.TryUnregisterGridInfo(_currentGridInfo);
            }
        }
    }
}