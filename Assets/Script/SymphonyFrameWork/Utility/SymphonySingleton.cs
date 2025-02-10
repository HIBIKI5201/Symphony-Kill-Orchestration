using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    public class SymphonySingleton : MonoBehaviour
    {
        [SerializeField]
        Component _target;

        private void OnEnable()
        {
            ServiceLocator.SetInstance(_target);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(_target);
        }
    }
}
