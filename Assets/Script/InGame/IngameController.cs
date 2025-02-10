using Orchestration.Entity;
using Orchestration.InGame;
using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameController : MonoBehaviour
    {
        UnitManager _unitManager;

        private void Start()
        {
            var controller = ServiceLocator.GetInstance<PlayerController>();

            //�����ꂽ��I�𒆂̕��m�Ɉړ��w��
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
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GridHighLight(hit.point);
            }


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

            if (controller)
            {
                controller.Active.OnStarted -= OnActive;
                controller.Select.OnStarted -= _unitManager.SelectSwitch;
            }
        }

        private void OnActive(float c)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _unitManager.SoldierMove(hit.point);
            }

        }

        /// <summary>
        /// �}�E�X�ɏd�Ȃ��Ă���O���b�h�̃n�C���C�g����
        /// </summary>
        private void GridHighLight(Vector3 point)
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();

            //�q�b�g�����ꏊ�̃O���b�h�ʒu��ڕW�n�_�ɃZ�b�g
            if (manager.GetGridByPosition(point, out GridInfo info))
            {

                manager.HighLightGrid(info);
            }
        }
    }
}
