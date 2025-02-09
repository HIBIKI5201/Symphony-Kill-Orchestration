using Orchestration.Entity;
using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using System.Collections.Generic;
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

        private void Start()
        {
            //äeï∫émÇê∂ê¨
            AddSoldier(SoldierType.Asult, new Vector3(0.5f, 0, -3.5f));
            AddSoldier(SoldierType.Support, new Vector3(0.5f, 0, -1.5f));
            AddSoldier(SoldierType.Medic, new Vector3(0.5f, 0, 1.5f));
            AddSoldier(SoldierType.Recon, new Vector3(0.5f, 0, 3.5f));

            //âüÇ≥ÇÍÇΩÇÁëIëíÜÇÃï∫émÇ…à⁄ìÆéwé¶
            var controller = ServiceLocator.GetInstance<PlayerController>();
            if (controller)
            {
                controller.Active.OnStarted += c => SelectSoldierMove();
            }

            _selectSolider = _soldiers[SoldierType.Asult];
        }

        private void SelectSoldierMove()
        {
            if (_selectSolider)
            {
                _selectSolider.SetDirection();
            }
            
        }

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

        public enum SoldierType
        {
            Asult,
            Medic,
            Support,
            Recon,
        }
    }
}
