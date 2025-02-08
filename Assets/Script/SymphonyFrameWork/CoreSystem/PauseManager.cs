using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SymphonyFrameWork.CoreSystem
{
    public static class PauseManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initiazlze()
        {
            _pause = false;
            OnPauseChanged = null;
        }

        private static bool _pause;
        public static bool Pause
        {
            get => _pause;
            set
            {
                _pause = value;
                OnPauseChanged?.Invoke(value);
            }
        }

        [Tooltip("ポーズ時にtrue、リズーム時にfalseで実行するイベント")]
        public static event Action<bool> OnPauseChanged;

        /// <summary>
        /// ポーズ時に停止するWaitForSecond
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static IEnumerator PausableWaitForSecond(float time)
        {
            while (time > 0)
            {
                if (!_pause)
                {
                    time -= Time.deltaTime;
                }
                yield return null;
            }
        }

        /// <summary>
        /// ポーズ時に停止するWaitForSecond
        /// </summary>
        /// <param name="time"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task PausableWaitForSecondAsync(float time, CancellationToken token = default)
        {
            while (time > 0)
            {
                if (!_pause)
                {
                    time -= Time.deltaTime;
                }
                await Awaitable.NextFrameAsync(token);
            }
        }

        public interface IPausable
        {
            void Pause();
            void Resume();

            /// <summary>
            /// PauseManagerにポーズ時のイベントを購買登録する
            /// </summary>
            /// <param name="pausable"></param>
            static void RegisterPauseManager(IPausable pausable)
            {
                OnPauseChanged += value =>
                {
                    if (value)
                    {
                        pausable.Pause();
                    }
                    else
                    {
                        pausable.Resume();
                    }
                };
            }
        }
    }
}