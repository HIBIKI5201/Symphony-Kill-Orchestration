using Orchestration.Entity;
using Orchestration.InGame;
using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameController : MonoBehaviour
    {
        private void Start()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            //押されたら選択中の兵士に移動指示
            var unitManager = ServiceLocator.GetInstance<UnitManager>();
            if (controller)
            {
                controller.Active.OnStarted += c => unitManager.SoldierMove();
                controller.Select.OnStarted += unitManager.SelectSwitch;
            }
        }

        private void Update()
        {
            GridHighLight();

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

            //押されたら選択中の兵士に移動指示
            var unitManager = ServiceLocator.GetInstance<UnitManager>();
            if (controller)
            {
                controller.Active.OnStarted -= c => unitManager.SoldierMove();
                controller.Select.OnStarted -= unitManager.SelectSwitch;
            }
        }

        /// <summary>
        /// マウスに重なっているグリッドのハイライトする
        /// </summary>
        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var manager = ServiceLocator.GetInstance<GroundManager>();

                //ヒットした場所のグリッド位置を目標地点にセット
                if (manager.GetGridByPosition(hit.point, out GridInfo info))
                {
                    manager.HighLightGrid(info);
                }
            }
        }
    }
}
