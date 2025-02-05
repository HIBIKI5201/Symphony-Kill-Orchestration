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
                //Enum�̖��O����O���[�v�����擾
                string name = type.ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                //�O���[�v���擾��AudioSource��������
                AudioMixerGroup group = _mixer.FindMatchingGroups(name).FirstOrDefault();
                if (group)
                {
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    source.outputAudioMixerGroup = group;

                    source.playOnAwake = false;

                    //�����̉��ʂ��擾
                    _mixer.GetFloat($"{name}_Volume", out float value);

                    //���������o�^
                    _audioDict.Add(type, (group, source, value));
                }
            }
        }

        /// <summary>
        /// Mixer�̉��ʂ�ύX����
        /// ���ʂ̓Q�[���J�n���̉��ʂ���Ŋ����ŕύX�����
        /// </summary>
        /// <param name="type">�ύX�������I�[�f�B�I�̎��</param>
        /// <param name="value">����</param>
        public void VolumeSliderChanged(AudioType type, float value)
        {
            if (value < 0 || 1 < value)
            {
                Debug.LogWarning("�^����ꂽ���ʂ͋K��l�O�ł�");
                return;
            }

            //��������X�V���ꂽ�f�V�x���P�ʂ̉��ʂ��v�Z����
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
