using Orchestration.InGame;
using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    /// <summary>
    /// ���m�̃x�[�X�N���X
    /// </summary>
    [RequireComponent(typeof(SoldierMove), typeof(SoldierUI))]
    public class SoldierManager : MonoBehaviour
    {
        [SerializeField]
        private SoldierData_SO _soldierData;

        private SoldierModel _model;

        private SoldierMove _move;
        private SoldierAttack _attack;

        private SoldierUI _ui;


        private void Awake()
        {
            var data = Instantiate(_soldierData);
            _soldierData = data;

            _model = GetComponent<SoldierModel>();

            _attack = GetComponent<SoldierAttack>();

            _move = GetComponent<SoldierMove>();

            _ui = GetComponent<SoldierUI>();

            if (_soldierData != null && _ui.NullCheckComponent($"{name}��UI��������܂���ł���"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / data.MaxHealthPoint);
                _soldierData.OnHealthChanged += OnDeath;
            }
        }

        private void Start()
        {
            _model.Init();
            _move.Init(_model.Agent);
            _ui.Init(_soldierData.Name);
        }

        private void Update()
        {
            //����̓v���C���[�̃}�l�[�W���[�Ɉړ��\��
            if (Input.GetMouseButtonDown(0))
            {
                _move.SetDirection(_model.Agent);
            }

            GridHighLight(); //����̓v���C���[�̃}�l�[�W���[�Ɉړ��\��


            //���m�̐��ʕ������`
            Vector3 forwardDirecion = Vector3.zero;
            float rotateTime = 3;

            //���͂ɓG������ꍇ�͍U���A���Ȃ��ꍇ�͈ړ�����������
            if (_attack.SearchTarget(_soldierData.AttackRange, _model.TargetLayer, out var enemy))
            {
                if (_attack.CanAttack(_soldierData.AttackInterval))
                {
                    _attack.AttackEnemy(enemy, _soldierData.Attack);
                    _model.Shoot();
                }

                //�G�̕����Ɍ���
                forwardDirecion = (enemy.transform.position - transform.position).normalized;
                rotateTime = 5;
            }
            else
            {
                forwardDirecion = _model.Agent.velocity.normalized;
            }
            _move.Rotation(forwardDirecion, rotateTime);

            //�ړ�
            _move.Move(_model.Agent, _model.Animator);

            //�w���X�o�[�̈ʒu�X�V
            _ui.HealthBarMove(transform.position, _model.HealthBarOffset);

            #region �f�o�b�O�@�\

            if (Input.GetMouseButtonDown(1))
            {
                var task = ServiceLocator.GetInstance<GridManager>().ChunkBuild();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ServiceLocator.GetInstance<GameLogic>().SceneChange(System.SceneEnum.InGame);
            }

            #endregion
        }

        /// <summary>
        /// �}�E�X�ɏd�Ȃ��Ă���O���b�h�̃n�C���C�g����
        /// </summary>
        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetInstance<GridManager>();

                //�q�b�g�����ꏊ�̃O���b�h�ʒu��ڕW�n�_�ɃZ�b�g
                if (gridManager.GetGridPosition(hit.point, out GridInfo info))
                {
                    gridManager.HighLightGrid(info);
                }
            }
        }

        /// <summary>
        /// ���m�Ƀ_���[�W��^����
        /// </summary>
        /// <param name="damage"></param>
        public void AddDamage(float damage) => _soldierData.HealthPoint -= damage;

        /// <summary>
        /// ���m�ɉ񕜂�^����
        /// </summary>
        /// <param name="heal"></param>
        public void AddHeal(float heal) => _soldierData.HealthPoint += heal;

        /// <summary>
        /// �w���X��0�ȉ��ɂȂ����玩�Ȕj�󏈗�����
        /// </summary>
        /// <param name="health"></param>
        private void OnDeath(float health)
        {
            if (health <= 0)
            {
                Destroy(gameObject);
            }
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

        #region ������OnGUI
        /*
        GUIStyle Style
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style = new GUIStyle();
                style.fontSize = 30;
                style.normal.textColor = Color.white;
                return style;
            }
        }

        /// <summary>
        /// ������OnGUI���g�p���Ă݂�
        /// </summary>
        private void OnGUI()
        {
            string[] logs = new string[2] { "a", "b" };

            float y = 10;
            foreach (string log in logs)
            {
                GUI.Label(new Rect(0, y, 350, 40), log, Style);
                y += 40;
            }
        }
        */
        #endregion
#endif
    }
}