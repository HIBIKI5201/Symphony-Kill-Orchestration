using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration
{
    public class HomeUIManager : MonoBehaviour
    {
        private UIDocument _document;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }
    }
}
