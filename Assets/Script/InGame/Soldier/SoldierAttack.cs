using SymphonyFrameWork.CoreSystem;
using System.Linq;
using UnityEngine;

namespace Orchestration.Entity
{
    public class SoldierAttack : MonoBehaviour
    {
        private float _attackTimer;

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
        /// <param name="damage"></param>
        public void AttackEnemy(SoldierManager target, float damage, SoldierManager me)
        {
            if (target)
            {
                target.AddDamage(damage, me);

                _attackTimer = Time.time; //インターバルをリセット
            }
        }
    }
}
