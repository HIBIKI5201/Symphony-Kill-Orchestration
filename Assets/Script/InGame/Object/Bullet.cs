using TMPro;
using UnityEngine;

namespace Orchestration
{
    public class Bullet : MonoBehaviour
    {
        private Transform _target;
        private float _speed;
        private Vector3 _offset;

        [SerializeField]
        private float _destroyDistance = 1;
        [SerializeField]
        private Vector3 _spread = new Vector3(0.2f, 0.3f, 0.2f);
        public void Init(float speed, Transform target, Vector3 offset)
        {
            _speed = speed;
            _target = target;
            _offset = offset;

            _offset += new Vector3(
                Random.Range(-_spread.x, _spread.x),
                Random.Range(-_spread.y, _spread.y),
                Random.Range(-_spread.z, _spread.z));

            Destroy(gameObject, 1);
        }

        private void Update()
        {
            if (!_target)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = (_target.position - transform.position + _offset).normalized;
            transform.position += dir * _speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, _target.position) < _destroyDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
