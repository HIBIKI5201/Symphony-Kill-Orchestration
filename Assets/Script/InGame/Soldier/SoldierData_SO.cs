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

        [Header("��{���")]

        [SerializeField]
        private Texture2D _icon;
        public Texture2D Icon { get => _icon; }

        [SerializeField]
        private string _name;
        public string Name
        {
            get
            {
                //���O���Ȃ���΃f�t�H���g����Ԃ�
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

        [Header("�̗̓X�e�[�^�X")]

        //�w���X
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

        //�X�^�~�i
        [SerializeField]
        private float _maxStaminaPoint = 100;
        public float MaxSpecialPoint { get => _maxStaminaPoint; }

        private int _specialPoint;
        public int SpecialPoint
        {
            get => _specialPoint;
            set
            {
                _specialPoint = value;
                OnSpecialPointChanged?.Invoke(_specialPoint);
            }
        }

        public event Action<int> OnSpecialPointChanged;

        [Space]

        [Header("�U���X�e�[�^�X")]

        [SerializeField]
        private float _attack = 10;
        public float Attack { get => _attack; }

        [SerializeField]
        private float _attackRatePerMinute = 600;
        public float AttackRatePerMinute { get => _attackRatePerMinute; }

        [SerializeField]
        private float _attackRange = 1;
        public float AttackRange { get => _attackRange; }
    }
}
