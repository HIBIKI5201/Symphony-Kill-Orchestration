using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;

namespace Orchestration.System
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer _mixer;

        private Dictionary<AudioType, (AudioMixerGroup group, AudioSource source, float originalVolume)> _audioDict = new();

        [SerializeField]
        private AudioVolumeSetting _webGLSetting;

#if UNITY_EDITOR
        [Space]

        [SerializeField]
        private AudioVolumeSetting _editorSetting;
#endif
        [Serializable]
        private struct AudioVolumeSetting
        {
            [Range(-80f, 20)]
            public float Master;

            [Range(-80f, 20)]
            public float BGM;

            [Range(-80f, 20)]
            public float SE;

            [Range(-80f, 20)]
            public float Voice;
        }

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
        }

        private void Awake()
        {
            AudioSourceInit();

            _audioDict[AudioType.BGM].source.loop = true;
        }

        private void Start()
        {
#if UNITY_EDITOR
            _mixer.SetFloat($"{AudioType.Master}_Volume", _editorSetting.Master);
            _mixer.SetFloat($"{AudioType.BGM}_Volume", _editorSetting.BGM);
            _mixer.SetFloat($"{AudioType.SE}_Volume", _editorSetting.SE);
            _mixer.SetFloat($"{AudioType.Voice}_Volume", _editorSetting.Voice);
#else
            _mixer.SetFloat($"{AudioType.Master}_Volume", _webGLSetting.Master);
            _mixer.SetFloat($"{AudioType.BGM}_Volume", _webGLSetting.BGM);
            _mixer.SetFloat($"{AudioType.SE}_Volume", _webGLSetting.SE);
            _mixer.SetFloat($"{AudioType.Voice}_Volume", _webGLSetting.Voice);
#endif
        }

        private void AudioSourceInit()
        {
            foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
            {
                //Enumの名前からグループ名を取得
                string name = type.ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                //グループを取得しAudioSourceを初期化
                AudioMixerGroup group = _mixer.FindMatchingGroups(name).FirstOrDefault();
                if (group)
                {
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    source.outputAudioMixerGroup = group;

                    source.playOnAwake = false;

                    //初期の音量を取得
                    _mixer.GetFloat($"{name}_Volume", out float value);

                    //情報を辞書登録
                    _audioDict.Add(type, (group, source, value));
                }
            }
        }

        /// <summary>
        /// Mixerの音量を変更する
        /// 音量はゲーム開始時の音量を基準で割合で変更される
        /// </summary>
        /// <param name="type">変更したいオーディオの種類</param>
        /// <param name="value">割合</param>
        public void VolumeSliderChanged(AudioType type, float value)
        {
            if (value < 0 || 1 < value)
            {
                Debug.LogWarning("与えられた音量は規定値外です");
                return;
            }

            //割合から更新されたデシベル単位の音量を計算する
            float db = value * (_audioDict[type].originalVolume + 80) - 80;

            _mixer.SetFloat(type.ToString(), db);
        }

        /// <summary>
        /// ミキサーグループを取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AudioMixerGroup GetMixerGroup(AudioType type) => _audioDict[type].group;
    }

    public enum AudioType
    {
        Master,
        BGM,
        SE,
        Voice,
    }
}
