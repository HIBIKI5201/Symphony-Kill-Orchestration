using UnityEngine;

namespace Orchestration.InGame
{
    /// <summary>
    /// �O���b�h�̏��Ƒ��������
    /// </summary>
    public class GridInfo : MonoBehaviour
    {
        private GameObject _highLight;

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
        }

        public void HighLightSetActive(bool value)
        {
            _highLight?.SetActive(value);
        }
    }
}
