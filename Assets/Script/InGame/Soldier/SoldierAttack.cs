using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Orchestration.Entity
{
    public class SoldierAttack : MonoBehaviour
    {
        private float _attackTimer;

        private List<Func<float, float>> _buffList = new();

        private void Update()
        {
            //ポーズ中はタイマーを保つ
            if (PauseManager.Pause)
            {
                _attackTimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// 攻撃できる状態かどうか
        /// </summary>
        /// <param name="rpm"></param>
        /// <returns></returns>
        public bool CanAttack(float rpm)
        {
            float interval = 60 / rpm;
            return interval + _attackTimer < Time.time;
        }

        /// <summary>
        /// 範囲内の兵士の中で最も近い者を取得する
        /// </summary>
        /// <param name="radius">探索半径</param>
        /// <param name="layerMask">攻撃対象のレイヤー</param>
        /// <param name="soldier"></param>
        /// <returns>範囲内に対象がいるかどうか</returns>
        public bool SearchTarget(float radius, LayerMask layerMask, out SoldierManager soldier)
        {
            soldier = default;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

            if (colliders.Length > 0)
            {
                SoldierManager[] soldiers = colliders
                    .Select(c => c.GetComponent<SoldierManager>())
                    .Where(sm => sm).ToArray();

                soldier = soldiers
                    //近い順番にソート
                    .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
                    //射線が通っているかを判定
                    .Where(s =>
                    {
                        //兵士に向けてRayを作成
                        Ray ray = new Ray(
                            transform.position + new Vector3(0, 1, 0),
                            (s.transform.position - transform.position).normalized
                            );

                        //当たったものがターゲットの兵士かどうか1
                        if (Physics.Raycast(ray, out var hitInfo))
                        {
                            if (hitInfo.transform == s.transform)
                            {
                                return true;
                            }
                        }
                        return false;
                    })
                    .FirstOrDefault();

                return soldier != null;
            }
            return false;
        }

        /// <summary>
        /// 対象を攻撃する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attackData"></param>
        public void AttackEnemy(SoldierManager target, AttackData attackData, SoldierManager me)
        {
            if (target)
            {
                float damage = attackData.Damage;

                if (attackData.IsCritical)
                {
                    damage *= 3;
                }

                //距離減衰
                float distance = Vector3.Distance(target.transform.position, me.transform.position);
                float rate = distance / me.Data.AttackRange;
                damage *= 1 - (1 - me.Data.DistanceDecay) * Mathf.Min(rate, 1);

                foreach (var buff in _buffList)
                {
                    damage = buff.Invoke(damage);
                }

                target.AddDamage(damage, me);

                _attackTimer = Time.time; //インターバルをリセット
            }
        }

        public void AddBuff(Func<float, float> func) => _buffList.Add(func);
        public void RemoveBuff(Func<float, float> func) => _buffList.Remove(func);
    }

    public struct AttackData
    {
        public float Damage;
        public bool IsCritical;
        public bool ActiveHighLight;

        public AttackData(float damage, bool isCritical = false, bool activeHighLight = false)
        {
            Damage = damage;
            IsCritical = isCritical;
            ActiveHighLight = activeHighLight;
        }
    }
}