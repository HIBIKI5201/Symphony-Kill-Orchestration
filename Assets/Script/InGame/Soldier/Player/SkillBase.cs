using UnityEngine;

namespace Orchestration.Entity
{
    public abstract class SkillBase : MonoBehaviour
    {
        public virtual void SkillVisible() { }

        public virtual void SkillActive(PlayerSoldierManager soldier, SoldierData_SO data)
        {   
            //�|�C���g������Ȃ���΃��^�[��
            if (data.SpecialPoint <= data.SpecialCost)
            {
                return;
            }

            if (SkillProccess(soldier, data))
            {
                data.SpecialPoint -= data.SpecialCost;
            }
        }

        /// <summary>
        /// �X�L���̏������s��
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="data"></param>
        /// <returns>�����������������ǂ���</returns>
        protected abstract bool SkillProccess(PlayerSoldierManager soldier, SoldierData_SO data);
    }
}
