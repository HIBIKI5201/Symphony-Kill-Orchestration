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

            //�����ꂽ��I�𒆂̕��m�Ɉړ��w��
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

            #region �f�o�b�O�@�\

            if (Input.GetKeyDown(KeyCode.Space))
            {
                PauseManager.Pause = !PauseManager.Pause;
            }

            #endregion
        }

        private void OnDestroy()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            //�����ꂽ��I�𒆂̕��m�Ɉړ��w��
            var unitManager = ServiceLocator.GetInstance<UnitManager>();
            if (controller)
            {
                controller.Active.OnStarted -= c => unitManager.SoldierMove();
                controller.Select.OnStarted -= unitManager.SelectSwitch;
            }
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
                if (manager.GetGridByPosition(hit.point, out GridInfo info))
                {
                    manager.HighLightGrid(info);
                }
            }
        }
    }
}
