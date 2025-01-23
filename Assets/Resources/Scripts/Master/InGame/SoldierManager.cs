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
                // 経路を取得
                NavMeshPath path = _agent.path;

                // 経路のコーナーポイントがあるか確認
                if (path.corners.Length < 2)
                {
                    return;
                }

                // 線の色を設定
                Gizmos.color = Color.blue;

                // 経路のコーナーポイントを線で結ぶ
                for (int i = 1; i < path.corners.Length; i++)
                {
                    Gizmos.DrawLine(path.corners[i - 1], path.corners[i]);
                }
            }
        }
#endif
    }
}