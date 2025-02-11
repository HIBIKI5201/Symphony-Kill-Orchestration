using Orchestration.Entity;
using UnityEngine;

namespace Orchestration.InGame
{
    /// <summary>
    /// �O���b�h�̏��Ƒ��������
    /// </summary>
    public class GridInfo : MonoBehaviour
    {
        [SerializeField]
        private GameObject _highLight;
        [SerializeField]
        private GameObject _ground;

        private MeshRenderer _groundRenderer;

        private bool _isActive;
        public bool IsActive { get => _isActive; set => _isActive = value; }

        private Vector3Int _position;
        public Vector3Int Position { get => _position; }

        private void Awake()
        {
            _isActive = true;

            if (_highLight)
            {
                _highLight.SetActive(false);
            }
            else
            {
                Debug.LogWarning("�O���b�h�̃n�C���C�g��������܂���");
            }

            if (_ground)
            {
                _groundRenderer = _ground.GetComponent<MeshRenderer>();
            }
            else
            {
                Debug.LogWarning("�O���b�h�̃O���E���h��������܂���");
            }
        }

        public void Init(Vector3 edge, float size)
        {
            _position = Vector3Int.FloorToInt(transform.position - edge);

            transform.localScale = Vector3.one * size;
        }

        public void HighLightSetActive(bool value)
        {
            _highLight?.SetActive(value);
        }

        public void GroundMaterialChange(Material material)
        {
            if (_groundRenderer)
            {
                Material[] materials = _groundRenderer.materials;
                materials[1] = material;
                _groundRenderer.materials = materials;
            }
        }
    }
}
