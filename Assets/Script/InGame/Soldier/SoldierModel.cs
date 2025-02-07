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
        #region コンポーネント
        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private Animator _animator;
        public Animator Animator { get => _animator; }
        #endregion

        [Header("Attack")]

        [SerializeField, Tooltip("攻撃したい兵士のレイヤー")]
        private LayerMask _targetLayer;
        public LayerMask TargetLayer { get => _targetLayer; }

        [Header("Animation")]

        [SerializeField, Tooltip("上半身のリグ")]
        private Rig _forwardRig;

        [SerializeField, Tooltip("上半身のリグのターゲット")]
        private Transform _forwardRigTarget;
        public Vector3 TargetPosition { get => _forwardRigTarget.position; }

        [Space]

        [SerializeField, Tooltip("マズルフラッシュのVFX")]
        private VisualEffect _muzzleFlash;
        public VisualEffect MuzzleFlash { get => _muzzleFlash; }

        [Header("Audio")]
        [SerializeField, Tooltip("マズルのオーディオソース")]
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
                //SE用のMixerGroupに指定
                _muzzleAudio.outputAudioMixerGroup = ServiceLocator.GetInstance<AudioManager>().GetMixerGroup(System.AudioType.SE);
                _muzzleAudio.playOnAwake = false;
            }

            if (_agent.NullCheckComponent("NavMeshAgentが見つかりません"))
            {
                //手動で動かすためアップデートはなし
                _agent.updatePosition = false;
                _agent.updateRotation = false;

                _agent.autoTraverseOffMeshLink = true;
            }

            if (_animator.NullCheckComponent("Animatorが見つかりません"))
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
