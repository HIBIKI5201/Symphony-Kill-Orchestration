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

        private MeshRenderer _groundRenderer;

        private bool _isActive;
        public bool IsActive { get => _isActive; set => _isActive = value; }

        private void Awake()
        {
            _isActive = true;

            if (_highLight)
            {
                _highLight.SetActive(false);
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
