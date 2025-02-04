using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

namespace Orchestration
{
    public class SoldierModel : MonoBehaviour
    {
        private NavMeshAgent _agent;

        [SerializeField]
        private Transform _target;
        public Vector3 TargetPosition { get => _target.position; }

        [SerializeField]
        private Rig _targetRig;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public Vector3 TargetDirection()
        {
            return _agent.velocity.normalized;
        }

        public void SetTargetRigWeight(float value)
        {
            _targetRig.weight = value;
        }

        public void Init()
        {
            if (!_target)
            {
                Debug.LogError("ターゲットが見つかりません");
            }

            if (!_targetRig)
            {
                Debug.LogError("ターゲットリグが見つかりません");
            }
        }
    }
}
