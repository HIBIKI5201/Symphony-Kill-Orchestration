using Orchestration.Entity;
using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class IngameController : MonoBehaviour
    {
        [SerializeField]
        private SoldierManager _selectSolider;

        private void Update()
        {
            //選んでいる兵士に移動指示を与える
            if (Input.GetMouseButtonDown(0))
            {
                if (_selectSolider)
                {
                    _selectSolider.SetDirection();
                }
            }

            GridHighLight();
        }

        /// <summary>
        /// マウスに重なっているグリッドのハイライトする
        /// </summary>
        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetInstance<GridManager>();

                //ヒットした場所のグリッド位置を目標地点にセット
                if (gridManager.GetGridPosition(hit.point, out GridInfo info))
                {
                    gridManager.HighLightGrid(info);
                }
            }
        }
    }
}
