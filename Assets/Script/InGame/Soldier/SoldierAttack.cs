using SymphonyFrameWork.CoreSystem;
using System.Linq;
using UnityEngine;

namespace Orchestration.Entity
{
    public class SoldierAttack : MonoBehaviour
    {
        private float _attackTimer;

        private void Update()
        {
            //�|�[�Y���̓^�C�}�[��ۂ�
            if (PauseManager.Pause)
            {
                _attackTimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// �U���ł����Ԃ��ǂ���
        /// </summary>
        /// <param name="rpm"></param>
        /// <returns></returns>
        public bool CanAttack(float rpm)
        {
            float interval = 60 / rpm;
            return interval + _attackTimer < Time.time;
        }

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

                soldier = soldiers
                    //�߂����ԂɃ\�[�g
                    .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
                    //�ː����ʂ��Ă��邩�𔻒�
                    .Where(s =>
                    {
                        //���m�Ɍ�����Ray���쐬
                        Ray ray = new Ray(
                            transform.position + new Vector3(0, 1, 0),
                            (s.transform.position - transform.position).normalized
                            );

                        //�����������̂��^�[�Q�b�g�̕��m���ǂ���1
                        if (Physics.Raycast(ray, out var hitInfo))
                        {
                            if (hitInfo.transform == s.transform)
                            {
                                return true;
                            }
                        }
                        return false;
                    })
                    .FirstOrDefault();

                return soldier != null;
            }
            return false;
        }

        /// <summary>
        /// �Ώۂ��U������
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public void AttackEnemy(SoldierManager target, float damage, SoldierManager me)
        {
            if (target)
            {
                target.AddDamage(damage, me);

                _attackTimer = Time.time; //�C���^�[�o�������Z�b�g
            }
        }
    }
}
