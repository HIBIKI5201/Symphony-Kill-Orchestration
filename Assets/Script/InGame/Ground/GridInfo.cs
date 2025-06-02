using Orchestration.Entity;
using UnityEngine;

namespace Orchestration.InGame
{
    /// <summary>
    /// グリッドの情報と操作を持つ
    /// </summary>
    public class GridInfo : MonoBehaviour
    {
        [SerializeField]
        private GameObject _highLight;
        [SerializeField]
        private GameObject _ground;

        private MeshRenderer _hightLightRenderer;
        private MeshRenderer _groundRenderer;

        [Space]

        [SerializeField]
        private Material _highLightMaterial;
        [SerializeField]
        private Material _highLightMaterialWarning;

        private bool _isUsed;
        public bool IsUsed { get => _isUsed; set => _isUsed = value; }

        private Vector3Int _position;
        public Vector3Int Position { get => _position; }

        private void Awake()
        {
            _isUsed = false;

            if (_highLight)
            {
                _highLight.SetActive(false);
                _hightLightRenderer = _highLight.GetComponent<MeshRenderer>();
            }
            else
            {
                Debug.LogWarning("グリッドのハイライトが見つかりません");
            }

            if (_ground)
            {
                _groundRenderer = _ground.GetComponent<MeshRenderer>();
            }
            else
            {
                Debug.LogWarning("グリッドのグラウンドが見つかりません");
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
            _hightLightRenderer.material =
                !_isUsed ? _highLightMaterial : _highLightMaterialWarning;
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
