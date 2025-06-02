using System;
using System.Collections.Generic;
using UnityEngine;

namespace Orchestration.Entity
{
    [CreateAssetMenu(fileName = "SoldierData", menuName = "GameDataSO/SoldierData")]
    public class SoldierData_SO : ScriptableObject
    {
        public void Awake()
        {
            //数値の初期化
            _healthPoint = _maxHealthPoint;
            _specialPoint = 0;
            _specialPointProportion = 0;

            //イベントを初期化
            OnHealthChanged = null;
            OnSpecialPointChanged = null;
            OnSpecialPointProportionChanged = null;
        }

        [Header("基本情報")]

        [SerializeField]
        private Sprite _icon;
        public Sprite Icon { get => _icon; }

        [SerializeField]
        private string _name;
        public string Name
        {
            get
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

        [Space]

        [SerializeField]
        private string _skillName;
        public string SkillName { get => _skillName; }

        [SerializeField, TextArea]
        private string _skillExplanation;
        public string SkillExplanation { get => _skillExplanation; }

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
                float amount = value;

                if (MaxHealthPoint < amount)
                {
                    amount = MaxHealthPoint;
                }

                _healthPoint = amount;
                OnHealthChanged?.Invoke(amount);
            }
        }
        public event Action<float> OnHealthChanged;

        [Space]

        [Header("攻撃ステータス")]

        [SerializeField]
        private float _attack = 10;
        public float Attack
        {
            get
            {
                float value = _attack;

                foreach (var buff in _attackBuffList)
                {
                    value = buff.Invoke(value);
                }

                return value;
            }
        }
        [SerializeField]
        private float _attackRatePerMinute = 600;
        public float AttackRatePerMinute { get => _attackRatePerMinute; }

        [SerializeField]
        private float _criticalChance = 5;
        public float CriticalChance { get => _criticalChance; }

        private List<Func<float, float>> _attackBuffList = new();

        public void AddAttackBuff(Func<float, float> func) => _attackBuffList.Add(func);
        public void RemoveAttackBuff(Func<float, float> func) => _attackBuffList.Remove(func);


        [Space]

        [SerializeField]
        private float _attackRange = 1;
        public float AttackRange { get => _attackRange; }

        [SerializeField]
        private float _distanceDecay = 0.8f;
        public float DistanceDecay { get => _distanceDecay; }

        [Header("スキル")]

        //スペシャルポイント
        [SerializeField]
        private int _skillCost = 10;
        public int SpecialCost { get => _skillCost; }

        [SerializeField]
        private int _maxSpecialPoint = 10;
        public int MaxSpecialPoint { get => _maxSpecialPoint; }

        private int _specialPoint;
        public int SpecialPoint
        {
            get => _specialPoint;
            set
            {
                int amount = value;

                if (MaxSpecialPoint < amount)
                {
                    amount = MaxSpecialPoint;
                }

                _specialPoint = amount;
                OnSpecialPointChanged?.Invoke(amount);
            }
        }

        public event Action<int> OnSpecialPointChanged;

        private float _specialPointProportion = 0;
        public float SpecialPointProportion
        {
            get => _specialPointProportion;
            set
            {
                float proportion = value;
                if (1 <= proportion)
                {
                    SpecialPoint++;
                    proportion -= 1;
                }

                OnSpecialPointProportionChanged?.Invoke(proportion);
                _specialPointProportion = proportion;
            }
        }

        public event Action<float> OnSpecialPointProportionChanged;
    }
}
