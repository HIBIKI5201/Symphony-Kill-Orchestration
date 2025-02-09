using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameSystemManager : MonoBehaviour
    {
        int _stageCounter = 0;

        GridManager _gridManager;
        private void Start()
        {
            _gridManager = ServiceLocator.GetInstance<GridManager>();
        }
    }
}
