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
            if (Physics.Raycast(ActiveRay, out RaycastHit hit, float.PositiveInfinity, _gridActiveLayer))
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
            if (Physics.Raycast(ActiveRay, out RaycastHit hit, float.PositiveInfinity, _gridActiveLayer))
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
