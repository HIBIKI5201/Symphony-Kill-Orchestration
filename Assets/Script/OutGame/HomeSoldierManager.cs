using Orchestration.Entity;
using Orchestration.OutGame;
using Orchestration.System;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class HomeSoldierManager : MonoBehaviour
    {
        [SerializeField]
        private SoldierModel _model;

        [SerializeField]
        private AudioSource _muzzleAudio;

        private void Start()
        {
            if (_model)
            {
                var system = ServiceLocator.GetInstance<OutGameSystemManager>();
                system.OnIngameLoad += GameStart;

                _model.Init();
            }
        }

        private void GameStart()
        {
            _model.Shoot();

            if (_muzzleAudio)
            {
                _muzzleAudio.playOnAwake = false;
                _muzzleAudio.spatialBlend = 1;
                _muzzleAudio.Play();
            }
        }
    }
}
