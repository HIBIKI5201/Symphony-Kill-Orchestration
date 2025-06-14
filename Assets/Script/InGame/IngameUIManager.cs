﻿using Orchestration.System;
using Orchestration.UI;
using SymphonyFrameWork.CoreSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.InGame
{
    public class IngameUIManager : MonoBehaviour
    {
        private UIDocument _document;

        private InputContext _inputContext;
        private MiniMap _miniMap;
        private UnitInfomation _unitInfo;
        private UnitSelector _unitSelector;
        private StageInfomation _stageInfo;
        private ResultWindow _resultWindow;
        private SkillInfomation _skillInfo;

        private Transform _miniMapCamera;

        [SerializeField]
        private float _miniMapCameraSpeed = 1;

        private float _miniMapFirstPosX;

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            if (_document)
            {
                VisualElement root = _document.rootVisualElement;
                _inputContext = root.Q<InputContext>();
                _miniMap = root.Q<MiniMap>();
                _unitInfo = root.Q<UnitInfomation>();
                _unitSelector = root.Q<UnitSelector>();
                _stageInfo = root.Q<StageInfomation>();
                _resultWindow = root.Q<ResultWindow>();
                _skillInfo = root.Q<SkillInfomation>();
            }

            _miniMapCamera = GetComponentInChildren<Camera>().transform;
            _miniMapFirstPosX = _miniMapCamera.position.x;
        }

        private void Start()
        {
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnStageChanged += MoveMiniMapCamera;
            system.OnStageChanged += CountUpdate;
            system.OnKillCounterChanged += KillCountUpdate;

            PlayerController controller = ServiceLocator.GetInstance<PlayerController>();
            controller.Input.OnStarted += OnInputStarted;
            controller.Input.OnCanseled += OnInputCanceled;

            CountUpdate(0);
            KillCountUpdate(0);
        }

        private void OnDestroy()
        {
            IngameSystemManager system = ServiceLocator.GetInstance<IngameSystemManager>();

            if (system)
            {
                system.OnStageChanged -= MoveMiniMapCamera;
                system.OnStageChanged -= CountUpdate;
                system.OnKillCounterChanged -= KillCountUpdate;
            }

            PlayerController controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Input.OnStarted -= OnInputStarted;
                controller.Input.OnCanseled -= OnInputCanceled;
            }
        }

        public void Add(VisualElement element) => _document.rootVisualElement.Add(element);

        public void AddSoldierInfo(UnitInfomationSoldier info) => _unitInfo?.AddSoldierInfo(info);
        public void AddSoldierSelector(UnitSelectorSoldier info) => _unitSelector?.AddSoldierInfo(info);

        public void CountUpdate(int count) => _stageInfo?.CountUpdate(count);
        public void KillCountUpdate(int count) => _stageInfo?.KillCountUpdate(count);

        public void SetSkillInfo(string name, string explanation) => _skillInfo?.SetSkillInfo(name, explanation);
        public async Task ResultWindowStart(int score, int stage, int kill) =>
            await _resultWindow.ResultWindowStart(score, stage, kill);

        private void OnInputStarted(float value) => _inputContext.ShowExplanation();
        private void OnInputCanceled(float value) => _inputContext.HideExplanation();

        private async void MoveMiniMapCamera(int count)
        {
            float nextPosX = count * GroundManager.ChunkSize + _miniMapFirstPosX;

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
