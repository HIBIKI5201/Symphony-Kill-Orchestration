using System;
using UnityEngine;

namespace Orchestration
{
    [CreateAssetMenu(fileName = "SoldierData", menuName = "GameDataSO/SoldierData")]
    public class SoldierData_SO : ScriptableObject
    {
        [SerializeField]
        private float _maxHealthPoint;
        public readonly float MaxHealthPoint;

        private float _healthPoint;
        public float HealthPoint 
        { 
            get => _healthPoint;
            set
            {
                _healthPoint = value;
                OnHealthChanged?.Invoke(value);
            }
        }
        public event Action<float> OnHealthChanged;

        [SerializeField]
        private float _maxStaminaPoint;
        public float MaxStaminaPoint { get => _maxStaminaPoint; }
    }
}
