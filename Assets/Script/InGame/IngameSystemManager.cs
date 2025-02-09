using SymphonyFrameWork.CoreSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Orchestration.InGame
{
    public class IngameSystemManager : MonoBehaviour
    {
        private int _stageCounter = 0;

        public event Action<int> OnStageChanged;

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        public void NextStage()
        {
            _stageCounter++;
            OnStageChanged?.Invoke(_stageCounter);
        }
    }
}
