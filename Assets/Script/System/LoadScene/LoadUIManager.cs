using Orchestration.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.System
{
    public class LoadUIManager : MonoBehaviour
    {
        private UIDocument _document;

        private LoadingUI _loadingUI;
        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = _document.rootVisualElement;

            _loadingUI = root.Q<LoadingUI>("LoadingUI");
        }

        public void ProgressUpdate(float progress) => _loadingUI.ProgressUpdate(progress);
    }
}
