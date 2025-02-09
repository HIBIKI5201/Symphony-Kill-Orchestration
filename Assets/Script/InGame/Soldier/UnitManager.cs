using NUnit.Framework;
using Orchestration.Entity;
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

        private List<PlayerSoldierManager> _soldiers = new();

        public GameObject GetSoldierPrefabByType(SoldierType soldierType)
        {
            return soldierType switch {
                SoldierType.Asult => _asult,
                SoldierType.Medic => _medic,
                SoldierType.Support => _support,
                SoldierType.Recon => _recon,
                _ => null,
            };
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
