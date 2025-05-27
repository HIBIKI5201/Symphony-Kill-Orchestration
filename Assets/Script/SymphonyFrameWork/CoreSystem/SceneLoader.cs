using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.CoreSystem
{
    public static class SceneLoader
    {
        private static Dictionary<string, Scene> _sceneDict = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void BeforeSceneLoad()
        {
            _sceneDict.Clear();
        }

        /// <summary>
        ///?Q?[???J?n???̏?????????
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            Scene scene = SceneManager.GetActiveScene();
            _sceneDict.Add(scene.name, scene);
        }

        /// <summary>
        /// ???[?h????Ă???V?[????Ԃ?
        /// ?Ȃ??ꍇ??null??Ԃ?
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool GetExistScene(string sceneName, out Scene scene) =>
            _sceneDict.TryGetValue(sceneName, out scene);

        public static bool SetActiveScene(string sceneName)
        {
            if (_sceneDict.TryGetValue(sceneName, out Scene scene))
            {
                SceneManager.SetActiveScene(scene);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ?V?[??????[?h????
        /// </summary>
        /// <param name="sceneName">?V?[????</param>
        /// <param name="loadingAction">???[?h?̐i??????????ɂ??????\?b?h</param>
        /// <returns>???[?h?ɐ?????????</returns>
        public static async Task<bool> LoadScene(string sceneName, Action<float> loadingAction = null)
        {
            if (_sceneDict.ContainsKey(sceneName))
            {
                Debug.LogWarning($"{sceneName}?V?[???͊??Ƀ??[?h????Ă??܂?");
                return false;
            }

            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (operation == null)
            {
                Debug.LogError($"{sceneName}?V?[???͓o?^????Ă??܂???");
                return false;
            }

            while (!operation.isDone)
            {
                loadingAction?.Invoke(operation.progress);
                await Awaitable.NextFrameAsync();
            }

            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                _sceneDict.TryAdd(sceneName, loadedScene);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// ?V?[????A?????[?h????
        /// </summary>
        /// <param name="sceneName">?V?[????</param>
        /// <param name="loadingAction">???[?h?̐i??????????ɂ??????\?b?h</param>
        /// <returns>?A?????[?h?ɐ?????????</returns>
        public static async Task<bool> UnloadScene(string sceneName, Action<float> loadingAction = null)
        {
            if (!_sceneDict.ContainsKey(sceneName))
            {
                Debug.LogWarning($"{sceneName}?V?[???̓??[?h????Ă??܂???");
                return false;
            }

            var operation = SceneManager.UnloadSceneAsync(sceneName);
            if (operation == null)
            {
                Debug.LogError($"{sceneName}?V?[???͓o?^????Ă??܂???");
                return false;
            }

            while (!operation.isDone)
            {
                loadingAction?.Invoke(operation.progress);
                await Awaitable.NextFrameAsync();
            }

            _sceneDict.Remove(sceneName);

            return true;
        }
    }
}
