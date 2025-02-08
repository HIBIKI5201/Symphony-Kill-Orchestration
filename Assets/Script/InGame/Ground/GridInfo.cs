using UnityEngine;

namespace Orchestration.InGame
{
    /// <summary>
    /// �O���b�h�̏��Ƒ��������
    /// </summary>
    public class GridInfo : MonoBehaviour
    {
        private GameObject _highLight;
        private GameObject _ground;

        private MeshRenderer _groundRenderer;

        private void Awake()
        {
            _highLight = gameObject.transform.Find("HighLight").gameObject;

            if (_highLight)
            {
                _highLight.SetActive(false);
            }
            else
            {
                Debug.LogWarning("�O���b�h�̃n�C���C�g��������܂���");
            }

            _ground = gameObject.transform.Find("Ground").gameObject;

            if (_ground)
            {
                _groundRenderer = _ground.GetComponent<MeshRenderer>();
            }
            else
            {
                Debug.LogWarning("�O���b�h�̃O���E���h��������܂���");
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
