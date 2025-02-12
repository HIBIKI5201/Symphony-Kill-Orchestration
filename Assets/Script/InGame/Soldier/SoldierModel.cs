using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.VFX;

namespace Orchestration.Entity
{
    public class SoldierModel : MonoBehaviour
    {
        #region �R���|�[�l���g
        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private Animator _animator;
        public Animator Animator { get => _animator; }

        private AudioSource _foodStepAudio;
        public AudioSource FoodStepAudio { get => _foodStepAudio; }
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

        [SerializeField]
        private GameObject _bulletPrefab;
        public GameObject BulletPrefab { get => _bulletPrefab; }

        [Header("UI")]

        [SerializeField]
        private Vector3 _healthBarOffset = Vector3.zero;
        public Vector3 HealthBarOffset { get => _healthBarOffset; }

        [SerializeField]
        private MeshRenderer _miniMapIcon;
        public MeshRenderer MiniMapIcon { get => _miniMapIcon; }

        [SerializeField]
        private Material _iconMaterial;
        public Material IconMaterial { get => _iconMaterial; }

        [SerializeField]
        private Material _selectedIconMaterial;
        public Material SelectedIconMaterial { get => _selectedIconMaterial; }

        [SerializeField]
        private LineRenderer _moveLineRenderer;
        public LineRenderer MoveLineRenderer { get => _moveLineRenderer; }
        public void Init()
        {
            _agent = GetComponent<NavMeshAgent>();

            _agent.enabled = false;

            _animator = GetComponentInChildren<Animator>();

            _moveLineRenderer = GetComponent<LineRenderer>();

            _foodStepAudio = GetComponent<AudioSource>();

            if (_miniMapIcon && _iconMaterial)
            {
                _miniMapIcon.material = _iconMaterial;
            }

            if (_muzzleAudio)
            {
                //SE�p��MixerGroup�Ɏw��
                _muzzleAudio.outputAudioMixerGroup = ServiceLocator.GetInstance<AudioManager>().GetMixerGroup(System.AudioType.SE);
                _muzzleAudio.playOnAwake = false;
                _muzzleAudio.spatialBlend = 1;
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

            if (_foodStepAudio.NullCheckComponent())
            {
                //SE�p��MixerGroup�Ɏw��
                _foodStepAudio.outputAudioMixerGroup = ServiceLocator.GetInstance<AudioManager>().GetMixerGroup(System.AudioType.SE);
                _foodStepAudio.playOnAwake = false;
                _foodStepAudio.Stop();
                _foodStepAudio.spatialBlend = 1;
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
                _muzzleAudio.Stop();
                _muzzleAudio.clip = _shootAudioClip;
                _muzzleAudio?.Play();
            }

            if (_animator)
            {
                _animator.SetTrigger("ShootTrigger");
            }
        }

        public void OnPause(bool pause)
        {
            if (pause)
            {
                _animator.speed = 0;
            }
            else
            {
                _animator.speed = 1;
            }
        }
    }
}
