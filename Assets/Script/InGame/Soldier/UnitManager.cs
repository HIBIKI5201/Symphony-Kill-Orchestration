using Orchestration.Entity;
using Orchestration.System;
using Orchestration.UI;
using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Orchestration.InGame
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _asult;
        [SerializeField]
        private GameObject _medic;
        [SerializeField]
        private GameObject _support;
        [SerializeField]
        private GameObject _recon;

        private Dictionary<SoldierType, PlayerSoldierManager> _soldiers = new();

        [SerializeField]
        private PlayerSoldierManager _selectSolider;

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        private void Start()
        {
            //�e���m�𐶐�
            AddSoldier(SoldierType.Asult, new Vector3(0.5f, 0, -3.5f));
            AddSoldier(SoldierType.Support, new Vector3(0.5f, 0, -1.5f));
            AddSoldier(SoldierType.Medic, new Vector3(0.5f, 0, 1.5f));
            AddSoldier(SoldierType.Recon, new Vector3(0.5f, 0, 3.5f));

            //�����ꂽ��I�𒆂̕��m�Ɉړ��w��
            var controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Active.OnStarted += c => SoldierMove();
                controller.Select.OnStarted += SelectSwitch;
            }

            var system = ServiceLocator.GetInstance<IngameSystemManager>();
            if (system)
            {
                system.OnStageChanged += BorderOutSoldierMove;
            }

            SelectorSoldier(_soldiers[SoldierType.Asult]);
        }

        private void Update()
        {
            var groundManager = ServiceLocator.GetInstance<GroundManager>();
            var system = ServiceLocator.GetInstance<IngameSystemManager>();

            foreach (var soldier in _soldiers.Values)
            {
                //���m��Next���E���ȏ�ɂ��邩�ǂ���
                if (soldier.transform.position.x > 
                    system.StageCounter * GroundManager.ChunkSize + groundManager.FirstBoundaryLineX
                    + GroundManager.ChunkSize * 2)
                {
                    system.NextStage();
                    break;
                }
            }
        }

        /// <summary>
        /// �I�𒆂̕��m��ύX
        /// </summary>
        /// <param name="axis"></param>
        private void SelectSwitch(float axis)
        {
            //�I�𒆂̕��m�̃C���f�b�N�X
            int index = _soldiers.Values.ToList().FindIndex(s => s == _selectSolider);
            
            axis = Math.Sign(axis);
            index = NextIndex(index, axis);

            SelectorSoldier(_soldiers.Values.ToArray()[index]);

            int NextIndex(int index, float axis)
            {
                index = Convert.ToInt32((index + axis) % _soldiers.Count);
                if (index < 0)
                {
                    index = _soldiers.Count - 1;
                }

                return index;
            }
        }

        private void SelectorSoldier(PlayerSoldierManager soldier)
        {
            //���̃Z���N�g��Ԃ�����
            if (_selectSolider)
            {
                _selectSolider.Select(false);
            }

            //�V���ɃZ���N�g��Ԃɂ���
            _selectSolider = soldier;
            _selectSolider.Select(true);

        }

        private void SoldierMove()
        {
            if (_selectSolider)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _selectSolider.SetDirection(hit.point);
                }
            }
        }

        private void BorderOutSoldierMove(int count)
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();

            float lineX = count * GroundManager.ChunkSize + manager.FirstBoundaryLineX;

            foreach (var soldier in _soldiers.Values)
            {
                if (soldier.transform.position.x <= lineX)
                {
                    //�ʒu�̉�������Ɉړ�����
                    Vector3 movePoint = soldier.transform.position;
                    movePoint.x = lineX;

                    GridInfo info = null;
                    do
                    {
                        //�ړ��悪������܂ŌJ��Ԃ�
                        if (movePoint.z < 5)
                        {
                            movePoint.z += 1;
                        }
                        else
                        {
                            movePoint.z = -10;
                            movePoint.x += 1;
                        }

                        //�O���b�h�����邩�m�F
                        manager.GetGridByPosition(movePoint, out info);
                    }
                    //�O���b�h�����邩�g�p�ς݂̏ꍇ�͌J��Ԃ�
                    while (info == null || manager.IsRegisterGridInfo(info));

                    soldier.SetDirection(info.transform.position);
                }
            }
        }

        /// <summary>
        /// �^�C�v����v���n�u��Ԃ�
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject GetSoldierPrefabByType(SoldierType type)
        {
            return type switch
            {
                SoldierType.Asult => _asult,
                SoldierType.Medic => _medic,
                SoldierType.Support => _support,
                SoldierType.Recon => _recon,
                _ => null,
            };
        }

        /// <summary>
        /// ���m�𐶐�����
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        public void AddSoldier(SoldierType type, Vector3 position)
        {
            GameObject prefab = GetSoldierPrefabByType(type);
            prefab = Instantiate(prefab, position, Quaternion.identity);
            prefab.transform.parent = transform;

            if (prefab.TryGetComponent<PlayerSoldierManager>(out var psm))
            {
                _soldiers.Add(type, psm);
            }
        }

        /// <summary>
        /// ���m�����S�������Ƃ��L�^����
        /// </summary>
        /// <param name="soldierManager"></param>
        public void DeathSoldier(PlayerSoldierManager soldierManager)
        {
            var type = _soldiers.ToList().Find(kvp => kvp.Value == soldierManager);

            //�����I�𒆂̕��m�Ȃ玟�̕��m�ɕύX
            if (type.Value == _selectSolider)
            {
                SelectSwitch(1);
            }

            _soldiers.Remove(type.Key);
        }

        public enum SoldierType
        {
            Asult,
            Medic,
            Support,
            Recon,
        }
    }
}
