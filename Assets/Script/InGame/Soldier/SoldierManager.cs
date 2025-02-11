using SymphonyFrameWork.CoreSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    /// <summary>
    /// ���m�̃x�[�X�N���X
    /// </summary>
    [RequireComponent(typeof(SoldierMove), typeof(SoldierUI), typeof(SoldierModel))]
    [RequireComponent(typeof(SoldierAttack))]

    public class SoldierManager : MonoBehaviour, PauseManager.IPausable
    {
        [SerializeField]
        protected SoldierData_SO _soldierData;

        protected SoldierModel _model;

        protected SoldierMove _move;
        protected SoldierAttack _attack;

        protected SoldierUI _ui;

        protected bool _isPause;

        private void Awake()
        {
            var data = Instantiate(_soldierData);
            _soldierData = data;

            _model = GetComponent<SoldierModel>();

            _attack = GetComponent<SoldierAttack>();

            _move = GetComponent<SoldierMove>();

            _ui = GetComponent<SoldierUI>();

            PauseManager.IPausable.RegisterPauseManager(this);

            Awake_S();
        }

        public virtual void Awake_S()
        {
            if (_soldierData != null)
            {
                _soldierData.OnHealthChanged += health =>
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
            Start_S();
        }

        protected virtual void Start_S()
        {
            _model.Init();
            _move.MoveGridPosition(_model.Agent);
            _ui.AddInfomationForHUD(_soldierData.Name, _soldierData.Icon);
        }

        private void Update()
        {
            //�|�[�Y���̏���
            if (_isPause)
            {
                _move.OnPauseMove(_model.Agent);
                return;
            }

            Update_S();
        }

        protected virtual void Update_S()
        {
            //�U���������������擾
            (Vector3 forwardDirecion, float rotateTime) = Attack();

            _move.Rotation(forwardDirecion, rotateTime);

            //�ړ�
            _move.Move(_model.Agent, _model.Animator);
        }

        /// <summary>
        /// �U������
        /// </summary>
        /// <returns>���������Ƒ��x</returns>
        protected virtual (Vector3, float) Attack()
        {
            //���͂ɓG������ꍇ�͍U���A���Ȃ��ꍇ�͈ړ�����������
            if (_attack.SearchTarget(_soldierData.AttackRange, _model.TargetLayer, out var enemy))
            {
                if (_attack.CanAttack(_soldierData.AttackRatePerMinute))
                {
                    _attack.AttackEnemy(enemy, _soldierData.Attack, this);
                    _model.Shoot();
                }

                //�����œG�̕����Ɍ���
                return ((enemy.transform.position - transform.position).normalized, 5);
            }
            else
            {
                //�������ړ������Ɍ���
                return (_model.Agent.velocity.normalized, 3);
            }
        }

        /// <summary>
        /// �ړ��ڕW���X�V
        /// </summary>
        public void SetDestination(Vector3 point)
        {
            _move.SetDestination(_model.Agent, point);
        }

        /// <summary>
        /// ���m�Ƀ_���[�W��^����
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="target">�U�������Ώ�</param>
        public virtual void AddDamage(float damage, SoldierManager target) => _soldierData.HealthPoint -= damage;

        /// <summary>
        /// ���m�ɉ񕜂�^����
        /// </summary>
        /// <param name="heal"></param>
        public virtual void AddHeal(float heal) => _soldierData.HealthPoint += heal;

        /// <summary>
        /// �w���X��0�ȉ��ɂȂ����玩�Ȕj�󏈗�����
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

            if (_soldierData)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, _soldierData.AttackRange);
            }
        }
#endif
    }
}