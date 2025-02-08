using Orchestration.InGame;
using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    /// <summary>
    /// 兵士のベースクラス
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

            if (_soldierData != null && _ui.NullCheckComponent($"{name}のUIが見つかりませんでした"))
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
            //これはプレイヤーのマネージャーに移動予定
            if (Input.GetMouseButtonDown(0))
            {
                _move.SetDirection(_model.Agent);
            }

            GridHighLight(); //これはプレイヤーのマネージャーに移動予定


            //兵士の正面方向を定義
            Vector3 forwardDirecion = Vector3.zero;
            float rotateTime = 3;

            //周囲に敵がいる場合は攻撃、いない場合は移動方向を向く
            if (_attack.SearchTarget(_soldierData.AttackRange, _model.TargetLayer, out var enemy))
            {
                if (_attack.CanAttack(_soldierData.AttackInterval))
                {
                    _attack.AttackEnemy(enemy, _soldierData.Attack);
                    _model.Shoot();
                }

                //敵の方向に向く
                forwardDirecion = (enemy.transform.position - transform.position).normalized;
                rotateTime = 5;
            }
            else
            {
                forwardDirecion = _model.Agent.velocity.normalized;
            }
            _move.Rotation(forwardDirecion, rotateTime);

            //移動
            _move.Move(_model.Agent, _model.Animator);

            //ヘルスバーの位置更新
            _ui.HealthBarMove(transform.position, _model.HealthBarOffset);

            #region デバッグ機能

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
        /// マウスに重なっているグリッドのハイライトする
        /// </summary>
        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetInstance<GridManager>();

                //ヒットした場所のグリッド位置を目標地点にセット
                if (gridManager.GetGridPosition(hit.point, out GridInfo info))
                {
                    gridManager.HighLightGrid(info);
                }
            }
        }

        /// <summary>
        /// 兵士にダメージを与える
        /// </summary>
        /// <param name="damage"></param>
        public void AddDamage(float damage) => _soldierData.HealthPoint -= damage;

        /// <summary>
        /// 兵士に回復を与える
        /// </summary>
        /// <param name="heal"></param>
        public void AddHeal(float heal) => _soldierData.HealthPoint += heal;

        /// <summary>
        /// ヘルスが0以下になったら自己破壊処理する
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

        #region お試しOnGUI
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
        /// 試しにOnGUIを使用してみた
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