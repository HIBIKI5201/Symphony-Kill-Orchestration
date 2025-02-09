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
    }
}
