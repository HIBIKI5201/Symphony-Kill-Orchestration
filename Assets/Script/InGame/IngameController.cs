using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameController : MonoBehaviour
    {
        UnitManager _unitManager;

        [SerializeField]
        private LayerMask _gridActiveLayer;

        private void Start()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            //押されたら選択中の兵士に移動指示
            _unitManager = ServiceLocator.GetInstance<UnitManager>();
            if (controller)
            {
                controller.Active.OnStarted += OnActive;
                controller.Select.OnStarted += _unitManager.SelectSwitch;
            }
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _gridActiveLayer))
            {
                GridHighLight(hit.point);
            }


            #region デバッグ機能

            if (Input.GetKeyDown(KeyCode.Space))
            {
                PauseManager.Pause = !PauseManager.Pause;
            }

            #endregion
        }

        private void OnDestroy()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                controller.Active.OnStarted -= OnActive;
                controller.Select.OnStarted -= _unitManager.SelectSwitch;
            }
        }

        private void OnActive(float c)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _gridActiveLayer))
            {
                _unitManager.SoldierMove(hit.point);
            }

        }

        /// <summary>
        /// マウスに重なっているグリッドのハイライトする
        /// </summary>
        private void GridHighLight(Vector3 point)
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();

            //ヒットした場所のグリッド位置を目標地点にセット
            if (manager.GetGridByPosition(point, out GridInfo info))
            {

                manager.HighLightGrid(info);
            }
        }
    }
}
