using TMPro;
using UnityEngine;

namespace Orchestration
{
    public class Bullet : MonoBehaviour
    {
        private Transform _target;
        private float _speed;
        private Vector3 _offset;

        public void Init(float speed, Transform target, Vector3 offset)
        {
            _speed = speed;
            _target = target;
            _offset = offset;

            _offset += new Vector3(
                Random.Range(-0.4f, 0.4f),
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.4f, 0.4f));
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

            if (Vector3.Distance(transform.position, _target.position) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }
}
