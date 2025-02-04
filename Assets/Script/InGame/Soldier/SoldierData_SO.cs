using System;
using UnityEngine;

namespace Orchestration.Entity
{
    [CreateAssetMenu(fileName = "SoldierData", menuName = "GameDataSO/SoldierData")]
    public class SoldierData_SO : ScriptableObject
    {
        [SerializeField]
        private string _name;
        public string Name { get => _name; }

        //ヘルス
        [SerializeField]
        private float _maxHealthPoint;
        public float MaxHealthPoint { get => _maxHealthPoint; }

        private float _healthPoint;
        public float HealthPoint
        { 
            get => _healthPoint;
            set
            {
                _healthPoint = value;
                OnHealthChanged?.Invoke(_healthPoint);
            }
        }
        public event Action<float> OnHealthChanged;

        //スタミナ
        [SerializeField]
        private float _maxStaminaPoint;
        public float MaxStaminaPoint { get => _maxStaminaPoint; }

        public void Awake()
        {
            _healthPoint = _maxHealthPoint;
        }
    }
}
