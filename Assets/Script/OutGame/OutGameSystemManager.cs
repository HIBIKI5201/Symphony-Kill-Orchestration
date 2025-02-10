using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration.OutGame
{
    public class OutGameSystemManager : MonoBehaviour
    {
        private void Start()
        {
            var audio = ServiceLocator.GetInstance<AudioManager>();
        }
    }
}
