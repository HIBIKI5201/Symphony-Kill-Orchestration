using System;
using UnityEngine;

namespace Orchestration.Entity
{
    [CreateAssetMenu(fileName = "SoldierData", menuName = "GameDataSO/SoldierData")]
    public class SoldierData_SO : ScriptableObject
    {
        public void Awake()
        {
            _healthPoint = _maxHealthPoint;
        }


        [SerializeField]
        private string _name;
        public string Name { get
            {
                //名前がなければデフォルト名を返す
                if (string.IsNullOrEmpty(_name))
                {
                    return "Soldier";
                }
                else
                {
                    return _name;
                }
            }
        }

        [Header("体力ステータス")]

        //ヘルス
        [SerializeField]
        private float _maxHealthPoint = 100;
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
        private float _maxStaminaPoint = 100;
        public float MaxSpecialPoint { get => _maxStaminaPoint; }

        private float _specialPoint;
        public float SpecialPoint
        {
            get => _specialPoint;
            set
            {
                _specialPoint = value;
                OnSpecialPointChanged?.Invoke(_specialPoint);
            }
        }

        public event Action<float> OnSpecialPointChanged;

        [Space]

        [Header("攻撃ステータス")]

        [SerializeField]
        private float _attackRange = 1;
        public float AttackRange { get => _attackRange; }
    }
}
