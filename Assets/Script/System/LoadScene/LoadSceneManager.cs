using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration
{
    public class LoadSceneManager : MonoBehaviour
    {
        UIDocument _ui;

        private void Awake()
        {
            _ui = GetComponent<UIDocument>();
        }

        public void ProgressBarUpdate(float progress)
        {

        }
    }
}
