using SymphonyFrameWork.CoreSystem;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    /// <summary>
    /// •ºm‚Ìƒx[ƒXƒNƒ‰ƒX
    /// </summary>
    [RequireComponent(typeof(SoldierMove), typeof(SoldierUI), typeof(SoldierModel))]
    [RequireComponent(typeof(SoldierAttack))]

    public class SoldierManager : MonoBehaviour, PauseManager.IPausable
    {
        [SerializeField]
        protected SoldierData_SO _data;
        public SoldierData_SO Data { get => _data; }

        protected SoldierModel _model;

        protected SoldierMove _move;
        protected SoldierAttack _attack;

        protected SoldierUI _ui;

        protected bool _isPause;

        private void OnEnable()
        {
            PauseManager.IPausable.RegisterPauseManager(this);
        }

        private void OnDisable()
        {
            PauseManager.IPausable.UnregisterPauseManager(this);
        }

        private void Awake()
        {
            var data = Instantiate(_data);
            _data = data;

            _model = GetComponent<SoldierModel>();

            _attack = GetComponent<SoldierAttack>();

            _move = GetComponent<SoldierMove>();

            _ui = GetComponent<SoldierUI>();

            Awake_S();
        }

        public virtual void Awake_S()
        {
            if (_data != null)
            {
                _data.OnHealthChanged += health =>
                {
                    if (health <= 0)
                    {
                        OnDeath();
                    }
                };
            }
        }


        private void Start()
        {
            _ui.MarkColorSet(_model.IconMaterial.color);

            Start_S();
        }

        protected virtual void Start_S()
        {
            _model.Init();
            _move.MoveGridPosition(_model.Agent);
            _ui.AddInfomationForHUD(_data.Name, _data.Icon);
        }

        private void Update()
        {
            //ƒ|[ƒY’†‚Ìˆ—
            if (_isPause)
            {
                _move.OnPauseMove(_model.Agent);
                return;
            }

            Update_S();
        }

        protected virtual void Update_S()
        {
            //UŒ‚‚µŒü‚­•ûŒü‚ğæ“¾
            (Vector3 forwardDirecion, float rotateTime) = Attack();

            _move.Rotation(forwardDirecion, rotateTime);

            //ˆÚ“®
            _move.Move(_model.Agent, _model.Animator, _model.FoodStepAudio);

            //ƒwƒ‹ƒXƒo[‚ÌˆÊ’uXV
            _ui.MarkMove(transform.position, _model.HealthBarOffset);
        }

        /// <summary>
        /// UŒ‚‚·‚é
        /// </summary>
        /// <returns>Œü‚­•ûŒü‚Æ‘¬“x</returns>
        protected virtual (Vector3, float) Attack()
        {
            //üˆÍ‚É“G‚ª‚¢‚éê‡‚ÍUŒ‚A‚¢‚È‚¢ê‡‚ÍˆÚ“®•ûŒü‚ğŒü‚­
            if (_attack.SearchTarget(_data.AttackRange, _model.TargetLayer, out var enemy))
            {
                AttackProccess(enemy);

                //‚‘¬‚Å“G‚Ì•ûŒü‚ÉŒü‚­
                return ((enemy.transform.position - transform.position).normalized, 5);
            }
            else
            {
                //‚ä‚Á‚­‚èˆÚ“®•ûŒü‚ÉŒü‚­
                return (_model.Agent.velocity.normalized, 3);
            }
        }

        protected virtual void AttackProccess(SoldierManager enemy)
        {
            //UŒ‚‚Å‚«‚È‚¢‚È‚çƒŠƒ^[ƒ“
            if (!_attack.CanAttack(_data.AttackRatePerMinute))
            {
                return;
            }

            float random = UnityEngine.Random.Range(0, 100);

            //“G‚Éƒ_ƒ[ƒW‚ğ—^‚¦‚é
            _attack.AttackEnemy(enemy, new(_data.Attack, isCritical: random < _data.CriticalChance), this);

            _model.Shoot();

            //’eŠÛ‚ğ¶¬‚·‚é
            if (_model.BulletPrefab)
            {
                var bullet = Instantiate(_model.BulletPrefab, _model.MuzzleFlash.transform.position, Quaternion.identity);
                bullet.GetComponent<Bullet>().Init(15, enemy.transform, new Vector3(0, 1, 0));
            }
        }

        public void AttackBuff(Func<float, float> func, bool active)
        {
            if (active)
            {
                _data.AddAttackBuff(func);
            }
            else
            {
                _data.RemoveAttackBuff(func);
            }
        }

        public void HealthBuff(Func<float, float> func, bool active)
        {
            if (active)
            {
                _data.AddHealthBuff(func);
            }
            else
            {
                _data.RemoveHealthBuff(func);
            }
        }

        /// <summary>
        /// ˆÚ“®–Ú•W‚ğXV
        /// </summary>
        public void SetDestination(Vector3 point)
        {
            _move.SetDestination(_model.Agent, point);
        }

        /// <summary>
        /// •ºm‚Éƒ_ƒ[ƒW‚ğ—^‚¦‚é
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="target">UŒ‚‚µ‚½‘ÎÛ</param>
        public virtual void AddDamage(AttackData data, SoldierManager target)
        {
            _ui.DamageTextInstantiate(data);
            _data.HealthPoint -= data.Damage;
        }

        /// <summary>
        /// •ºm‚É‰ñ•œ‚ğ—^‚¦‚é
        /// </summary>
        /// <param name="heal"></param>
        public virtual void AddHeal(float heal) => _data.HealthPoint += heal;

        /// <summary>
        /// ƒwƒ‹ƒX‚ª0ˆÈ‰º‚É‚È‚Á‚½‚ç©ŒÈ”j‰óˆ—‚·‚é
        /// </summary>
        /// <param name="health"></param>
        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        public void Pause()
        {
            _isPause = true;
            _model.OnPause(true);
        }

        public void Resume()
        {
            _isPause = false;
            _model.OnPause(false);
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            NavMeshAgent agent = _model?.Agent;

            if (agent != null && agent.path != null)
            {
                NavMeshPath path = agent.path;

                if (path.corners.Length < 2)
                {
                    return;
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawLineStrip(path.corners, false);
            }

            if (_data)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, _data.AttackRange);
            }
        }
#endif
    }
}