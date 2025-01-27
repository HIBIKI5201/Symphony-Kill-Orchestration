using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{
    /// <summary>
    /// 兵士のベースクラス
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class SoldierManager : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Animator _animator;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent)
            {
                _agent.updatePosition = false;
                _agent.updateRotation = false;
            }

            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (_agent != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 hitPosition = hit.point;
                        _agent.SetDestination(hitPosition);
                    }
                }

                Vector3 direction = (_agent.nextPosition - transform.position).normalized;

                // nextPositionからdeltaPositionを算出
                var worldDeltaPosition = _agent.nextPosition - transform.position;

                // キャラクターを基点にしたxz平面に射影したdeltaPosition
                var dx = Vector3.Dot(transform.right, worldDeltaPosition);
                var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
                Vector2 deltaPosition = new Vector2(dx, dy);

                // Time.deltaTimeから速度を算出
                var velocity = deltaPosition / Time.deltaTime;

                Vector2 duration = deltaPosition.normalized;
                _animator.SetFloat("Right", duration.x);
                _animator.SetFloat("Forward", duration.y);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_agent != null && _agent.path != null)
            {
                NavMeshPath path = _agent.path;

                if (path.corners.Length < 2)
                {
                    return;
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawLineStrip(path.corners, false);
            }
        }

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
#endif
    }
}