using Orchestration.Entity;
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
        private GameObject _assault;
        [SerializeField]
        private GameObject _medic;
        [SerializeField]
        private GameObject _support;
        [SerializeField]
        private GameObject _recon;

        private Dictionary<SoldierType, PlayerSoldierManager> _soldiers = new();

        private PlayerSoldierManager _selectSolider;

        public PlayerSoldierManager[] UnitSoldiers { get => _soldiers.Values.ToArray(); }
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
            AddSoldier(SoldierType.Assault, new Vector3(0.5f, 0, -3.5f));
            AddSoldier(SoldierType.Support, new Vector3(0.5f, 0, -1.5f));
            AddSoldier(SoldierType.Medic, new Vector3(0.5f, 0, 1.5f));
            AddSoldier(SoldierType.Recon, new Vector3(0.5f, 0, 3.5f));

            var system = ServiceLocator.GetInstance<IngameSystemManager>();
            if (system)
            {
                system.OnStageChanged += BorderOutSoldierMove;
            }

            SelectorSoldier(_soldiers[SoldierType.Assault]);
        }

        private void Update()
        {
            var groundManager = ServiceLocator.GetInstance<GroundManager>();
            var system = ServiceLocator.GetInstance<IngameSystemManager>();


            foreach (var soldier in _soldiers.Values)
            {
                //兵士がNext境界線以上にいるかどうか
                if (soldier.transform.position.x >
                    system.StageCounter * GroundManager.ChunkSize
                    + groundManager.FirstBoundaryLineX
                    + GroundManager.ChunkSize * (GroundManager.NextBoundaryLineCount - 1))
                {
                    system.NextStage();
                    break;
                }
            }

        }

        /// <summary>
        /// 選択中の兵士を変更
        /// </summary>
        /// <param name="axis"></param>
        public void SelectSwitch(float axis)
        {
            //選択中の兵士のインデックス
            int index = _soldiers.Values.ToList().FindIndex(s => s == _selectSolider);

            axis = Math.Sign(axis);
            index = NextIndex(index, axis);

            SelectorSoldier(_soldiers.Values.ToArray()[index]);

            int NextIndex(int index, float axis)
            {
                index = (int)(index + axis) % _soldiers.Count;
                if (index < 0)
                {
                    index = _soldiers.Count - 1;
                }

                return index;
            }
        }

        private void SelectorSoldier(PlayerSoldierManager soldier)
        {
            if (!soldier)
            {
                return;
            }

            //元のセレクト状態を解除
            if (_selectSolider)
            {
                _selectSolider.Select(false);
            }

            //新たにセレクト状態にする
            _selectSolider = soldier;
            _selectSolider.Select(true);

        }

        /// <summary>
        /// 兵士をカメラの正面方向に移動させる
        /// </summary>
        public void SoldierMove(Vector3 point)
        {
            if (_selectSolider)
            {
                _selectSolider.SetDestination(point);
            }
        }

        public void SkillActive(float value) => _selectSolider.SkillActive();

        private void BorderOutSoldierMove(int count)
        {
            var manager = ServiceLocator.GetInstance<GroundManager>();
            if (manager)
            {
                return;
            }

            float lineX = count * GroundManager.ChunkSize + manager.FirstBoundaryLineX;

            foreach (var soldier in _soldiers.Values)
            {
                if (soldier.AgentDestination.x <= lineX)
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

                    soldier.SetDestination(info.transform.position);
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
                SoldierType.Assault => _assault,
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
            var type = _soldiers.ToList().Find(kvp => kvp.Value == soldierManager);

            //もし選択中の兵士なら次の兵士に変更
            if (type.Value == _selectSolider)
            {
                SelectSwitch(1);
            }

            _soldiers.Remove(type.Key);

            if (_soldiers.Count < 1)
            {
                var system = ServiceLocator.GetInstance<IngameSystemManager>();
                if (system)
                {
                    system.ResultOpen();
                }
            }
        }

        public enum SoldierType
        {
            Assault,
            Medic,
            Support,
            Recon,
        }
    }
}
