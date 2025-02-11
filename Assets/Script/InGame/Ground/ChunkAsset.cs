using Orchestration.Entity;
using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class ChunkAsset : MonoBehaviour
    {
        [SerializeField]
        GameObject _enemy;

        [SerializeField]
        GameObject _object;

        private int _enemyValue;

        public int EnemyValue { get => _enemyValue; }
        private void Awake()
        {
            var enemies = _enemy.GetComponentsInChildren<EnemySoliderManager>();
            _enemyValue = enemies.Length;
        }
        private void OnDestroy()
        {
            var system = ServiceLocator.GetInstance<IngameSystemManager>();

            if (system)
            {
                system.AddAcviveEnemy(EnemyValue);
            }
        }
    }
}

