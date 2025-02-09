using Orchestration.Entity;
using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.InGame
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

            #region �f�o�b�O�@�\

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
        /// �}�E�X�ɏd�Ȃ��Ă���O���b�h�̃n�C���C�g����
        /// </summary>
        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var manager = ServiceLocator.GetInstance<GroundManager>();

                //�q�b�g�����ꏊ�̃O���b�h�ʒu��ڕW�n�_�ɃZ�b�g
                if (manager.GetGridPosition(hit.point, out GridInfo info))
                {
                    manager.HighLightGrid(info);
                }
            }
        }
    }
}
