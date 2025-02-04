using Orchestration.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration
{
    public class IngameUIManager : MonoBehaviour
    {
        private UIDocument _document;

        private MiniMap _miniMap;
        private UnitInfomation _unitInfo;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            VisualElement root = _document.rootVisualElement;
            _miniMap = root.Q<MiniMap>();
            _unitInfo = root.Q<UnitInfomation>();
        }
    }
}
