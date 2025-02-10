using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private List<AudioClip> _bgmList = new();
        private CancellationTokenSource _bgmChangeToken;

        [Space]

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

        /// <summary>
        /// �~�L�T�[�O���[�v���擾
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AudioMixerGroup GetMixerGroup(AudioType type) => _audioDict[type].group;

        public void BGMChanged(int index, float duration)
        {
            //�L�����Z������Ă��Ȃ���Ύ~�߂�
            if (!_bgmChangeToken.IsCancellationRequested)
            {
                _bgmChangeToken.Cancel();
            }

            _bgmChangeToken = new();
            var token = _bgmChangeToken.Token;

            //BGM���t�F�[�h���鏈��
            Task task = new Task(async () =>
            {
                AudioSource source = _audioDict[AudioType.BGM].source;
                AudioClip bgm = _bgmList[index];

                //���ʂ����Ȃ��Ȃ�܂ő҂�
                while (source.volume > 0)
                {
                    source.volume -= duration / 2 * Time.time;
                    await Awaitable.NextFrameAsync();
                }

                source.volume = 0;

                //�N���b�v�����ւ�
                source.Stop();
                source.clip = bgm;
                source.Play();

                //���ʂ��傫���Ȃ�܂ő҂�
                while (source.volume < 1)
                {
                    source.volume += duration / 2 * Time.time;
                    await Awaitable.NextFrameAsync();
                }

                source.volume = 1;

            }, token);
        }

        #region OnGUI

        private GUIStyle Style
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style = new GUIStyle();
                style.fontSize = 30;
                style.normal.textColor = Color.white;
                return style;
            }
        }

        /// <summary>
        /// ������OnGUI���g�p���Ă݂�
        /// </summary>
        private void OnGUI()
        {
            List<string> logs = new();

            foreach (string name in Enum.GetNames(typeof(AudioType)))
            {
                if (_mixer.GetFloat($"{name}_Volume", out float value))
                {
                    logs.Add($"{name} volume : {value}");
                }
            }

            float y = 10;
            foreach (string log in logs)
            {
                GUI.Label(new Rect(0, y, 350, 40), log, Style);
                y += 40;
            }
        }

        #endregion
    }

    public enum AudioType
    {
        Master,
        BGM,
        SE,
        Voice,
    }
}
