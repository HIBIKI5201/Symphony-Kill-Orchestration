using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Orchestration
{
    public class SoldierModel : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        public Vector3 TargetPosition { get => _target.position; }

        [SerializeField]
        private Rig _targetRig;

        public void SetTargetRigWeight(float value)
        {
            _targetRig.weight = value;
        }

        public void Init()
        {
            if (!_target)
            {
                Debug.LogError("�^�[�Q�b�g��������܂���");
            }

            if (!_targetRig)
            {
                Debug.LogError("�^�[�Q�b�g���O��������܂���");
            }
        }
    }
}
