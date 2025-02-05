using Codice.CM.SEIDInfo;
using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

namespace Orchestration.System
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer _mixer;

        private Dictionary<AudioType, (AudioMixerGroup group, AudioSource source, float originalVolume)> _audioDict = new();

        private void Awake()
        {
            AudioSourceInit();

            _audioDict[AudioType.BGM].source.loop = true;
        }

        private void OnEnable()
        {
            ServiceLocator.SetInstance(this);
        }

        private void OnDisable()
        {
            ServiceLocator.DestroyInstance(this);
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
    }

    public enum AudioType
    {
        Master,
        BGM,
        SE,
        Voice,
    }
}
