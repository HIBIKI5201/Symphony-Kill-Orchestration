using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

namespace Orchestration.Entity
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

            _ui = GetComponent<SoldierUI>();

            if (_model.NullCheckComponent($"{name}のモデルが見つかりませんでした") && _ui.NullCheckComponent($"{name}のUIが見つかりませんでした"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / data.MaxHealthPoint);
                _soldierData.OnHealthChanged += value => Debug.Log(value);
            }
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

        private void AddDamage(float damage) => _soldierData.HealthPoint -= damage;
        private void AddHeal (float heal) => _soldierData.HealthPoint += heal;

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