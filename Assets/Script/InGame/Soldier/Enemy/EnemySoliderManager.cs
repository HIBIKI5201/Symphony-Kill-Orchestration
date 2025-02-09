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
            //�U���������������擾
            (Vector3 forwardDirecion, float rotateTime) = Attack();

            _move.Rotation(forwardDirecion, rotateTime);

            //�ړ�
            _move.Move(_model.Agent, _model.Animator);
        }
    }
}
