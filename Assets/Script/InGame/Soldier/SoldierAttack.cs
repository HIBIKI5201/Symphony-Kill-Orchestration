using Orchestration.Entity;
using System.Linq;
using UnityEngine;

namespace Orchestration
{
    public class SoldierAttack : MonoBehaviour
    {
        /// <summary>
        /// �͈͓��̕��m�̒��ōł��߂��҂��擾����
        /// </summary>
        /// <param name="radius">�T�����a</param>
        /// <param name="layerMask"></param>
        /// <param name="soldier"></param>
        /// <returns>�͈͓��ɑΏۂ����邩�ǂ���</returns>
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
        /// �Ώۂ��U������
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
