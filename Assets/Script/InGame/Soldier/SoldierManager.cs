using Orchestration.InGame;
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

        private SoldierUI _ui;

        private void Awake()
        {
            var data = Instantiate(_soldierData);
            _soldierData = data;

            _model = GetComponent<SoldierModel>();

            _move = GetComponent<SoldierMove>();

            _ui = GetComponent<SoldierUI>();

            if (_soldierData != null && _ui.NullCheckComponent($"{name}のUIが見つかりませんでした"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / data.MaxHealthPoint);
            }
        }

        private void Start()
        {
            _model.Init();

            _move.Init();

            string name = _soldierData.Name;

            _ui.Init(name);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _move.SetDirection();
            }

            GridHighLight();

            Vector3 direction = _model.TargetDirection();
            _move.Rotation(direction);

            _move.Move();

            _ui.HealthBarMove(transform.position);

            #region デバッグ機能

            if (Input.GetMouseButtonDown(1))
            {
                var task = ServiceLocator.GetSingleton<GridManager>().ChunkBuild();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ServiceLocator.GetSingleton<GameLogic>().SceneChange(System.SceneEnum.InGame);
            }

            #endregion
        }

        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetSingleton<GridManager>();

                //ヒットした場所のグリッド位置を目標地点にセット
                if (gridManager.GetGridPosition(hit.point, out Vector3 pos, out int index))
                {
                    gridManager.HighLightGrid(index);
                }
            }
        }

        private void AddDamage(float damage) => _soldierData.HealthPoint -= damage;
        private void AddHeal(float heal) => _soldierData.HealthPoint += heal;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            NavMeshAgent agent = _move?.Agent;

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