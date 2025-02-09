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
            //UŒ‚‚µŒü‚­•ûŒü‚ğæ“¾
            (Vector3 forwardDirecion, float rotateTime) = Attack();

            _move.Rotation(forwardDirecion, rotateTime);

            //ˆÚ“®
            _move.Move(_model.Agent, _model.Animator);
        }
    }
}
