using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.OutGame
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
