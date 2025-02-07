using Orchestration.UI;
using SymphonyFrameWork.CoreSystem;
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
            if (_document)
            {
                VisualElement root = _document.rootVisualElement;
                _miniMap = root.Q<MiniMap>();
                _unitInfo = root.Q<UnitInfomation>();
            }
        }

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        private void Start()
        {

        }

        public void AddSoldierInfo(UnitInfomationSoldier info) => _unitInfo?.AddSoldierInfo(info);
    }
}
