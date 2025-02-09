using Orchestration.Entity;
using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameController : MonoBehaviour
    {

        private void Update()
        {
            GridHighLight();

            #region デバッグ機能

            if (Input.GetMouseButtonDown(1))
            {
                ServiceLocator.GetInstance<IngameSystemManager>().NextStage();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                PauseManager.Pause = !PauseManager.Pause;
            }

            #endregion
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
