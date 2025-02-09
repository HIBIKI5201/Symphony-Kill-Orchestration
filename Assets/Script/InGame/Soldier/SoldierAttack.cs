using Orchestration.Entity;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Orchestration.Entity
{
    public class SoldierAttack : MonoBehaviour
    {
        private float _attackTimer;

        /// <summary>
        /// 攻撃のインターバルが終了しているかどうか
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool CanAttack(float interval) => interval + _attackTimer < Time.time;

        /// <summary>
        /// 範囲内の兵士の中で最も近い者を取得する
        /// </summary>
        /// <param name="radius">探索半径</param>
        /// <param name="layerMask">攻撃対象のレイヤー</param>
        /// <param name="soldier"></param>
        /// <returns>範囲内に対象がいるかどうか</returns>
        public bool SearchTarget(float radius, LayerMask layerMask, out SoldierManager soldier)
        {
            soldier = default;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

            if (colliders.Length > 0)
            {
                SoldierManager[] soldiers = colliders
                    .Select(c => c.GetComponent<SoldierManager>())
                    .Where(sm => sm).ToArray();

                soldier = soldiers.OrderBy(s => Vector3.Distance(transform.position, s.transform.position)).FirstOrDefault();
                return soldier != null;
            }
            return false;
        }

        /// <summary>
        /// 対象を攻撃する
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="damage"></param>
        public void AttackEnemy(SoldierManager soldier, float damage)
        {
            if (soldier)
            {
                soldier.AddDamage(damage);

                _attackTimer = Time.time; //インターバルをリセット
            }
        }
    }
}
