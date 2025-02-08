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
            //�I��ł��镺�m�Ɉړ��w����^����
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
        /// �}�E�X�ɏd�Ȃ��Ă���O���b�h�̃n�C���C�g����
        /// </summary>
        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetInstance<GridManager>();

                //�q�b�g�����ꏊ�̃O���b�h�ʒu��ڕW�n�_�ɃZ�b�g
                if (gridManager.GetGridPosition(hit.point, out GridInfo info))
                {
                    gridManager.HighLightGrid(info);
                }
            }
        }
    }
}
