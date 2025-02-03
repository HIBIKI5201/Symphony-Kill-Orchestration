using Orchestration.System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orchestration.System
{
    public class LoadSceneManager : MonoBehaviour
    {
        private LoadUIManager _loadUIManager;

        private void Awake()
        {
            _loadUIManager = GetComponent<LoadUIManager>();
        }

        public void ProgressUpdate(float progress) => _loadUIManager.ProgressUpdate(progress);
    }
}
