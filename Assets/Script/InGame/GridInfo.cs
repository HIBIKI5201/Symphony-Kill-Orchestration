using UnityEngine;

namespace Orchestration.InGame
{
    /// <summary>
    /// グリッドの情報と操作を持つ
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
                Debug.LogWarning("グリッドのハイライトが見つかりません");
            }
        }

        public void HighLightSetActive(bool value)
        {
            _highLight?.SetActive(value);
        }
    }
}
