using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
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

        [Header("Audio")]
        [SerializeField, Tooltip("�}�Y���̃I�[�f�B�I�\�[�X")]
        private AudioSource _muzzleAudio;

        [SerializeField]
        private AudioClip _shootAudioClip;
        public AudioClip ShootAudioClip { get => _shootAudioClip; }

        [Header("UI")]

        [SerializeField]
        private Vector3 _healthBarOffset = Vector3.zero;
        public Vector3 HealthBarOffset { get => _healthBarOffset; }


        public void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();

            if (_muzzleAudio)
            {
                //SE�p��MixerGroup�Ɏw��
                _muzzleAudio.outputAudioMixerGroup = ServiceLocator.GetInstance<AudioManager>().GetMixerGroup(System.AudioType.SE);
                _muzzleAudio.playOnAwake = false;
            }

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

        public void Shoot()
        {
            if (_muzzleFlash)
            {
                _muzzleFlash.Play();
            }

            if (_shootAudioClip)
            {
                _muzzleAudio?.PlayOneShot(_shootAudioClip);
            }

            if (_animator)
            {
                _animator.SetTrigger("ShootTrigger");
            }
        }
    }
}
