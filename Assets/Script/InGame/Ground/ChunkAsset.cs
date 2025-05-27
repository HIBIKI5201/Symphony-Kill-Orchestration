using Orchestration.Entity;
using UnityEngine;

namespace Orchestration
{
    public class ChunkAsset : MonoBehaviour
    {
        [SerializeField] private GameObject _enemy;

        [SerializeField] private GameObject _object;

        public int EnemyValue { get; private set; }

        private void Awake()
        {
            var enemies = _enemy.GetComponentsInChildren<EnemySoliderManager>();
            EnemyValue = enemies.Length;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            var selfPos = transform.position;

            for (var i = -_gridLength.x / 2; i <= _gridLength.x / 2; i++)
            for (var j = -_gridLength.y / 2; j <= _gridLength.y / 2; j++)
            {
                var from = selfPos + new Vector3(i, 0, j) * _gridSize;

                Gizmos.DrawLine(from, from + Vector3.up * _lineLength);
            }
        }
#endif

#if UNITY_EDITOR
        [Header("Gizmo設定")] [SerializeField] private Vector2 _gridLength = Vector2.one * 5;
        [SerializeField] private float _gridSize = 5;
        [Space] [SerializeField] private float _lineLength = 3;
#endif
    }
}