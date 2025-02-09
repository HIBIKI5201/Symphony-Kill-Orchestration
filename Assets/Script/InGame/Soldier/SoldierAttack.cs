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
        /// �U���̃C���^�[�o�����I�����Ă��邩�ǂ���
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool CanAttack(float interval) => interval + _attackTimer < Time.time;

        /// <summary>
        /// �͈͓��̕��m�̒��ōł��߂��҂��擾����
        /// </summary>
        /// <param name="radius">�T�����a</param>
        /// <param name="layerMask">�U���Ώۂ̃��C���[</param>
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

                _attackTimer = Time.time; //�C���^�[�o�������Z�b�g
            }
        }
    }
}
