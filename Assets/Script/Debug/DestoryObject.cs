using UnityEngine;

namespace Orchestration
{
    public class DestoryObject : MonoBehaviour
    {
        [SerializeField]
        private float _destroyDeferment = 0;

        void Start() => Destroy(gameObject, _destroyDeferment);
    }
}
