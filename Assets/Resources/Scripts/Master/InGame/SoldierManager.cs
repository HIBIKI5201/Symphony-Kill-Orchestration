using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.InGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SoldierManager : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent)
            {
                _agent.autoTraverseOffMeshLink = true;
            }
        }

        private void Update()
        {
            if (_agent != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 hitPosition = hit.point;
                    _agent.SetDestination(hitPosition);
                }
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
        /// ŽŽ‚µ‚ÉOnGUI‚ðŽg—p‚µ‚Ä‚Ý‚½
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