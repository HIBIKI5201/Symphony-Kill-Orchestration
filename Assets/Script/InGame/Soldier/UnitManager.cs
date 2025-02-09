using Orchestration.Entity;
using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        private SoldierManager _selectSolider;

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
            //各兵士を生成
            AddSoldier(SoldierType.Asult, new Vector3(0.5f, 0, -3.5f));
            AddSoldier(SoldierType.Support, new Vector3(0.5f, 0, -1.5f));
            AddSoldier(SoldierType.Medic, new Vector3(0.5f, 0, 1.5f));
            AddSoldier(SoldierType.Recon, new Vector3(0.5f, 0, 3.5f));

            //押されたら選択中の兵士に移動指示
            var controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Active.OnStarted += c => SelectSoldierMove();
            }

            var system = ServiceLocator.GetInstance<IngameSystemManager>();
            if (system) {
                system.OnStageChanged += BorderOutSoldierMove;
                    }

            _selectSolider = _soldiers[SoldierType.Asult];
        }

        private void SelectSoldierMove()
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

            float lineX = count * GroundManager.ChunkSize + manager.FirstBoudaryLineX;

            foreach (var soldier in _soldiers.Values)
            {
                if (soldier.transform.position.x <= lineX)
                {
                    //位置の延長線上に移動する
                    Vector3 movePoint = soldier.transform.position;
                    movePoint.x = lineX;

                    GridInfo info = null;
                    do
                    {
                        //移動先が見つかるまで繰り返す
                        if (movePoint.z < 5)
                        {
                            movePoint.z += 1;
                        }
                        else
                        {
                            movePoint.z = -10;
                            movePoint.x += 1;
                        }

                        //グリッドがあるか確認
                        manager.GetGridByPosition(movePoint, out info);
                    }
                    //グリッドがあるか使用済みの場合は繰り返す
                    while (info == null || manager.IsRegisterGridInfo(info));

                    soldier.SetDirection(info.transform.position);
                }
            }
        }

        /// <summary>
        /// タイプからプレハブを返す
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
        /// 兵士を生成する
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
        /// 兵士が死亡したことを記録する
        /// </summary>
        /// <param name="soldierManager"></param>
        public void DeathSoldier(PlayerSoldierManager soldierManager)
        {
            SoldierType type = _soldiers.ToList().Find(kvp => kvp.Value == soldierManager).Key;
            _soldiers.Remove(type);
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
