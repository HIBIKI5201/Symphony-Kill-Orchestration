using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.VFX;

namespace Orchestration.Entity
{
    public class SoldierModel : MonoBehaviour
    {
        #region コンポーネント
        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private Animator _animator;
        public Animator Animator { get => _animator; }

        private AudioSource _foodStepAudio;
        public AudioSource FoodStepAudio { get => _foodStepAudio; }
        private bool _isFoodStepPlayed;
        #endregion

        [Header("攻撃")]

        [SerializeField, Tooltip("攻撃したい兵士のレイヤー")]
        private LayerMask _targetLayer;
        public LayerMask TargetLayer { get => _targetLayer; }

        [Header("演出")]

        [SerializeField, Tooltip("上半身のリグ")]
        private Rig _forwardRig;

        [SerializeField, Tooltip("上半身のリグのターゲット")]
        private Transform _forwardRigTarget;
        public Vector3 TargetPosition { get => _forwardRigTarget.position; }

        [Space]

        [SerializeField, Tooltip("マズルフラッシュのVFX")]
        private VisualEffect _muzzleFlash;
        public VisualEffect MuzzleFlash { get => _muzzleFlash; }

        [SerializeField]
        private Light _muzzleFlashLight;
        private Task _muzzleFlashLightTask;

        [SerializeField]
        private VisualEffect _shellVFX;

        [SerializeField]
        private GameObject _bulletPrefab;
        public GameObject BulletPrefab { get => _bulletPrefab; }

        [Header("オーディオ")]
        [SerializeField, Tooltip("マズルのオーディオソース")]
        private AudioSource _muzzleAudio;

        [SerializeField]
        private AudioClip _shootAudioClip;

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
            if (_agent)
            {
                _agent.enabled = false;
            }

            _animator = GetComponentInChildren<Animator>();

            _moveLineRenderer = GetComponent<LineRenderer>();

            _foodStepAudio = GetComponent<AudioSource>();

            if (_miniMapIcon && _iconMaterial)
            {
                _miniMapIcon.material = _iconMaterial;
            }

            if (_muzzleAudio)
            {
                //SE用のMixerGroupに指定
                _muzzleAudio.outputAudioMixerGroup = ServiceLocator.GetInstance<AudioManager>().GetMixerGroup(System.AudioType.SE);
                _muzzleAudio.playOnAwake = false;
                _muzzleAudio.spatialBlend = 1;
            }

            if (_muzzleFlashLight)
            {
                _muzzleFlashLight.enabled = false;
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

            if (_foodStepAudio.NullCheckComponent())
            {
                //SE用のMixerGroupに指定
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

            if (_muzzleAudio && _shootAudioClip)
            {
                _muzzleAudio.Stop();
                _muzzleAudio.clip = _shootAudioClip;
                _muzzleAudio?.Play();
            }

            if (_animator)
            {
                _animator.SetTrigger("ShootTrigger");
            }

            if (_shellVFX)
            {
                _shellVFX.Play();
            }

            if (_muzzleFlashLight)
            {
                //まだライトが付いているならスキップ
                if (_muzzleFlashLightTask != null && !_muzzleFlashLightTask.IsCompleted)
                {
                    goto MuzzleFlashEnd;
                }

                _muzzleFlashLightTask = LightUp(destroyCancellationToken);

                async Task LightUp(CancellationToken token)
                {
                    const float interval = 0.025f;
                    try
                    {
                        _muzzleFlashLight.enabled = true;
                        await Awaitable.WaitForSecondsAsync(interval, token);
                    }
                    finally
                    {
                        _muzzleFlashLight.enabled = false;
                        await Awaitable.WaitForSecondsAsync(interval, token);
                    }
                }

            MuzzleFlashEnd:;
            }
        }

        public void OnPause(bool pause)
        {
            if (pause)
            {
                _animator.speed = 0;

                //もし歩きサウンドが再生中だったら状態を保存して停止
                if (_foodStepAudio.isPlaying)
                {
                    _isFoodStepPlayed = true;
                    _foodStepAudio.Stop();
                }
            }
            else
            {
                _animator.speed = 1;

                //もし歩きサウンドが再生していたら再開
                if (_isFoodStepPlayed)
                {
                    _isFoodStepPlayed = false;
                    _foodStepAudio.Play();
                }
            }
        }
    }
}
