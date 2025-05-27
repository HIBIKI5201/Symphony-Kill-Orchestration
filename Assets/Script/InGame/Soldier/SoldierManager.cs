using SymphonyFrameWork.CoreSystem;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    /// <summary>
    /// 兵士のベースクラス
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
            //ポーズ中の処理
            if (_isPause)
            {
                _move.OnPauseMove(_model.Agent);
                return;
            }

            Update_S();
        }

        protected virtual void Update_S()
        {
            //攻撃し向く方向を取得
            (Vector3 forwardDirecion, float rotateTime) = Attack();

            _move.Rotation(forwardDirecion, rotateTime);

            //移動
            _move.Move(_model.Agent, _model.Animator, _model.FoodStepAudio);

            //ヘルスバーの位置更新
            _ui.MarkMove(transform.position, _model.HealthBarOffset);
        }

        /// <summary>
        /// 攻撃する
        /// </summary>
        /// <returns>向く方向と速度</returns>
        protected virtual (Vector3, float) Attack()
        {
            //周囲に敵がいる場合は攻撃、いない場合は移動方向を向く
            if (_attack.SearchTarget(_data.AttackRange, _model.TargetLayer, out var enemy))
            {
                AttackProccess(enemy);

                //高速で敵の方向に向く
                return ((enemy.transform.position - transform.position).normalized, 5);
            }
            else
            {
                //ゆっくり移動方向に向く
                return (_model.Agent.velocity.normalized, 3);
            }
        }

        protected virtual void AttackProccess(SoldierManager enemy)
        {
            //攻撃できないならリターン
            if (!_attack.CanAttack(_data.AttackRatePerMinute))
            {
                return;
            }

            float random = UnityEngine.Random.Range(0, 100);

            //敵にダメージを与える
            _attack.AttackEnemy(enemy, new(_data.Attack, isCritical: random < _data.CriticalChance), this);

            _model.Shoot();

            //弾丸を生成する
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

        /// <summary>
        /// 移動目標を更新
        /// </summary>
        public void SetDestination(Vector3 point)
        {
            _move.SetDestination(_model.Agent, point);
        }

        /// <summary>
        /// 兵士にダメージを与える
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="target">攻撃した対象</param>
        public virtual void AddDamage(AttackData data, SoldierManager target)
        {
            _ui.DamageTextInstantiate(data);
            _data.HealthPoint -= data.Damage;
        }

        /// <summary>
        /// 兵士に回復を与える
        /// </summary>
        /// <param name="heal"></param>
        public virtual void AddHeal(float heal) => _data.HealthPoint += heal;

        /// <summary>
        /// ヘルスが0以下になったら自己破壊処理する
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