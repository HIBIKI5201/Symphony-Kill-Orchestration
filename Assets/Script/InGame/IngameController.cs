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

        private Ray ActiveRay
        {
            get
            {
                return Camera.main.ScreenPointToRay(Input.mousePosition);
            }
        }

        private void Start()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            //押されたら選択中の兵士に移動指示
            _unitManager = ServiceLocator.GetInstance<UnitManager>();
            if (controller)
            {
                controller.Active.OnStarted += OnActive;
                controller.Select.OnStarted += _unitManager.SelectSwitch;
                controller.Skill.OnStarted += _unitManager.SkillActive;
            }

            //リザルト時の操作形態に変更
            var system = ServiceLocator.GetInstance<IngameSystemManager>();
            system.OnResultOpen += ResultWindowOpen;
            system.OnResultEnd += ResultWindowEnd;
        }

        private void Update()
        {
            if (Physics.Raycast(ActiveRay, out RaycastHit hit, float.PositiveInfinity, _gridActiveLayer))
            {
                GridHighLight(hit.point);
            }
        }

        private void OnActive(float c)
        {
            if (Physics.Raycast(ActiveRay, out RaycastHit hit, float.PositiveInfinity, _gridActiveLayer))
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

        private void ResultWindowOpen()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                controller.Active.OnStarted -= OnActive;
                controller.Select.OnStarted -= _unitManager.SelectSwitch;
                controller.Skill.OnStarted -= _unitManager.SkillActive;
            }
        }

        public void ResultWindowEnd()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            if (controller)
            {
                controller.Select.OnStarted += ResultCotrol;
            }
        }

        private void ResultCotrol(float c)
        {
            var logic = ServiceLocator.GetInstance<GameLogic>();

            if (0 < c)
            {
                logic.SceneChange(SceneEnum.Home);
            }
            else
            {
                logic.SceneChange(SceneEnum.InGame);
            }

            var controller = ServiceLocator.GetInstance<PlayerController>();
            controller.Select.OnStarted -= ResultCotrol;
        }

        private void OnDrawGizmos()
        {
            if (Camera.main)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(ActiveRay.origin, ActiveRay.origin + (ActiveRay.direction * 1000));
            }
        }
    }
}
