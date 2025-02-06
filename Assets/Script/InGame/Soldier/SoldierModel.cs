using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.VFX;

namespace Orchestration
{
    public class SoldierModel : MonoBehaviour
    {
        #region �R���|�[�l���g
        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private Animator _animator;
        public Animator Animator { get => _animator; }
        #endregion

        [Header("Attack")]

        [SerializeField, Tooltip("�U�����������m�̃��C���[")]
        private LayerMask _targetLayer;
        public LayerMask TargetLayer { get => _targetLayer; }

        [Header("Animation")]

        [SerializeField, Tooltip("�㔼�g�̃��O")]
        private Rig _forwardRig;

        [SerializeField, Tooltip("�㔼�g�̃��O�̃^�[�Q�b�g")]
        private Transform _forwardRigTarget;
        public Vector3 TargetPosition { get => _forwardRigTarget.position; }

        [Space]

        [SerializeField, Tooltip("�}�Y���t���b�V����VFX")]
        private VisualEffect _muzzleFlash;
        public VisualEffect MuzzleFlash { get => _muzzleFlash; }

        [Header("UI")]

        [SerializeField]
        private Vector3 _healthBarOffset = Vector3.zero;
        public Vector3 HealthBarOffset { get => _healthBarOffset; }


        public void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();

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

        public void SetTargetRigWeight(float value)
        {
            _forwardRig.weight = value;
        }
    }
}
