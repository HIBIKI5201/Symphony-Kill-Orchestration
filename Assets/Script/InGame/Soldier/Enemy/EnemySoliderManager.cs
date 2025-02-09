using UnityEngine;

namespace Orchestration.Entity
{
    public class EnemySoliderManager : SoldierManager
    {
        protected override void Start_S()
        {
            _model.Init();
            _move.MoveGridPosition(_model.Agent);
        }

        protected override void Update_S()
        {
            //攻撃し向く方向を取得
            (Vector3 forwardDirecion, float rotateTime) = Attack();

            _move.Rotation(forwardDirecion, rotateTime);

            //移動
            _move.Move(_model.Agent, _model.Animator);
        }
    }
}
