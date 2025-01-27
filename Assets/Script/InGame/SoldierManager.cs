using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{
    /// <summary>
    /// 兵士のベースクラス
    /// </summary>
    [RequireComponent(typeof(SoldierModel))]
    public class SoldierManager : MonoBehaviour
    {
        private SoldierModel _model;

        //移動系プロパティ
        private Vector2 _currentDirection;
        private void Awake()
        {
            _model = GetComponent<SoldierModel>();
            _model.NullCheckComponent($"{name}のモデルが見つかりませんでした");
        }

        private void Update()
        {
            NavMeshAgent agent = _model.Agent;

            if (agent != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 hitPosition = hit.point;
                        agent.SetDestination(hitPosition);
                    }
                }

                #region Move

                Vector3 localNextPos = transform.InverseTransformPoint(agent.nextPosition);
                Vector2 targetDirection = new Vector2(localNextPos.x, localNextPos.z).normalized;

                //Lerpで滑らかに変化
                _currentDirection = Vector2.Lerp(_currentDirection, targetDirection, Time.deltaTime * 5);

                Animator animator = _model.Animator;
                animator.SetFloat("Right", _currentDirection.x);
                animator.SetFloat("Forward", _currentDirection.y);

                transform.position = agent.nextPosition;

                #endregion

                Vector3 direction = _model.Target.position - transform.position;
                direction.y = 0;  // Y軸方向を無視

                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Lerpで滑らかに変化
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 5);
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