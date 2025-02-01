using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{
    /// <summary>
    /// 兵士のベースクラス
    /// </summary>
    [RequireComponent(typeof(SoldierModel), typeof(SoldierUI))]
    public class SoldierManager : MonoBehaviour
    {
        [SerializeField]
        private SoldierData_SO _soldierData;

        private SoldierModel _model;

        private SoldierUI _ui;

        private void Awake()
        {
            var data = Instantiate(_soldierData);
            _soldierData = data;

            _model = GetComponent<SoldierModel>();
            _model.NullCheckComponent($"{name}のモデルが見つかりませんでした");

            _ui = GetComponent<SoldierUI>();
            _ui.NullCheckComponent($"{name}のUIが見つかりませんでした");
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _model.SetDirection();
            }

            _model.Move();

            _ui.HealthBarMove(transform.position);
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
        }

        #region お試しOnGUI
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
        #endregion
#endif
    }
}