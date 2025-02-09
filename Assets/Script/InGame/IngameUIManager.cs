using Orchestration.UI;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.InGame
{
    public class IngameUIManager : MonoBehaviour
    {
        private UIDocument _document;

        private MiniMap _miniMap;
        private UnitInfomation _unitInfo;
        private UnitSelector _unitSelector;

        private Transform _miniMapCamera;

        [SerializeField]
        private float _miniMapCameraSpeed = 1;

        private float _miniMapFirstPosX;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            if (_document)
            {
                VisualElement root = _document.rootVisualElement;
                _miniMap = root.Q<MiniMap>();
                _unitInfo = root.Q<UnitInfomation>();
                _unitSelector = root.Q<UnitSelector>();
            }

            _miniMapCamera = GetComponentInChildren<Camera>().transform;
            _miniMapFirstPosX = _miniMapCamera.position.x;
        }

        private void Start()
        {
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += MoveMiniMapCamera;
        }

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        public void AddSoldierInfo(UnitInfomationSoldier info) => _unitInfo?.AddSoldierInfo(info);
        public void AddSoldierSelector(UnitSelectorSoldier info) => _unitSelector?.AddSoldierInfo(info);

        private async void MoveMiniMapCamera(int count)
        {
            float nextPosX = count * 10 + _miniMapFirstPosX;

            //次のステージ位置に移動するまで繰り返す
            while (nextPosX >= _miniMapCamera.position.x)
            {
                _miniMapCamera.position += new Vector3(_miniMapCameraSpeed * Time.deltaTime, 0, 0);

                await Awaitable.NextFrameAsync();
            }

            //移動完了したら整数値に戻す
            _miniMapCamera.position = new Vector3(nextPosX, _miniMapCamera.position.y, _miniMapCamera.position.z);
        }
    }
}
