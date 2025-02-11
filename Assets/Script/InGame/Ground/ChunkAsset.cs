using Orchestration.Entity;
using UnityEngine;

namespace Orchestration
{
    public class ChunkAsset : MonoBehaviour
    {
        [SerializeField]
        GameObject _enemy;

        [SerializeField]
        GameObject _object;

        public int EnemyValue
        {
            get
            {
                var enemies = _enemy.GetComponentsInChildren<EnemySoliderManager>();
                return enemies.Length;
            }
        }
    }
}

