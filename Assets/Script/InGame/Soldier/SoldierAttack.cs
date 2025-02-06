using Orchestration.Entity;
using System.Linq;
using UnityEngine;

namespace Orchestration
{
    public class SoldierAttack : MonoBehaviour
    {
        /// <summary>
        /// ”ÍˆÍ“à‚Ì•ºm‚Ì’†‚ÅÅ‚à‹ß‚¢Ò‚ğæ“¾‚·‚é
        /// </summary>
        /// <param name="radius">’Tõ”¼Œa</param>
        /// <param name="layerMask"></param>
        /// <param name="soldier"></param>
        /// <returns>”ÍˆÍ“à‚É‘ÎÛ‚ª‚¢‚é‚©‚Ç‚¤‚©</returns>
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
        /// ‘ÎÛ‚ğUŒ‚‚·‚é
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="damage"></param>
        public void AttackEnemy(SoldierManager soldier, float damage)
        {
            if (soldier)
            {
                soldier.AddDamage(damage);
            }
        }
    }
}
