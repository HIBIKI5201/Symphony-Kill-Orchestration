using SymphonyFrameWork.CoreSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Orchestration.System
{
    public class SceneChanger
    {
        private string _currentSceneName;

        private bool _isLoading;
        public bool IsLoading { get => _isLoading; }

        private float _fadeOutTime = 1;
        private float _fadeInTime = 1;

        public SceneChanger(float fadeOutTime, float fadeInTime)
        {
            Scene scene = SceneManager.GetActiveScene();
            _currentSceneName = scene.name;
            _fadeOutTime = fadeOutTime;
            _fadeInTime = fadeInTime;
        }

        public async void SceneLoad(SceneEnum sceneEnum)
        {
            if (_isLoading)
            {
                Debug.LogWarning("???[?f?B???O???Ƀ??[?h?ł??܂???");
                return;
            }
            _isLoading = true;
            PauseManager.Pause = true;

            await FadeOut(_fadeOutTime);

            //???[?h?V?[???̃??[?h
            if (!await SceneLoader.LoadScene(SceneEnum.LoadingScene.ToString()))
            {
                Debug.LogError("???[?h?V?[????ǂݍ??߂܂???ł???");
                return;
            }
            else
            {
                //???[?h?V?[????A?N?e?B?u??
                SceneLoader.GetExistScene(SceneEnum.LoadingScene.ToString(), out Scene scene);
                SceneManager.SetActiveScene(scene);
            }

            //???[?h?V?[???̃}?l?[?W???[??擾????
            LoadSceneManager manager = null;

            if (SceneLoader.GetExistScene(SceneEnum.LoadingScene.ToString(), out Scene loadScene))
            {
                //???[?h?V?[???̃??[?g?I?u?W?F?N?g???烍?[?h?V?[???}?l?[?W???[??T??
                foreach (GameObject obj in loadScene.GetRootGameObjects())
                {
                    manager = obj.GetComponent<LoadSceneManager>();
                    if (manager)
                    {
                        break;
                    }
                }

                if (!manager)
                {
                    Debug.LogError("???[?h?V?[???}?l?[?W???[????????܂???");
                    LoadFailed();
                    return;
                }
            }

            await FadeIn(_fadeInTime);

            //???̃V?[????A?????[?h
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentSceneName.ToString(),
                progress =>
                {
                    manager.ProgressUpdate(progress / 2); //0?`50%?͈̔?
                });

            //?????P?\????
            await Awaitable.WaitForSecondsAsync(0.5f);

            if (unloadSuccess)
            {
                //?V?[??????[?h
                bool loadSuccess = await SceneLoader.LoadScene(sceneEnum.ToString(),
                    progress =>
                    {
                        manager.ProgressUpdate(progress / 2 + 0.5f); //50?`100%?͈̔?
                    });

                if (loadSuccess)
                {
                    //???[?h?ɐ????????猻?݂̃V?[????A?N?e?B?u?????ċL??????
                    if (SceneLoader.GetExistScene(sceneEnum.ToString(), out Scene scene))
                    {
                        SceneManager.SetActiveScene(scene);
                        _currentSceneName = sceneEnum.ToString();
                    }
                }
                else
                {
                    LoadFailed();
                    return;
                }
            }
            else return;

            //???????????𖾎????邽?߂ɃQ?[?W??ő剻
            manager.ProgressUpdate(1);

            //?????P?\????
            await Awaitable.WaitForSecondsAsync(0.5f);

            await FadeOut(_fadeOutTime);

            //???[?h?V?[????A?????[?h
            await SceneLoader.UnloadScene(SceneEnum.LoadingScene.ToString());

            await FadeIn(_fadeInTime);

            PauseManager.Pause = false;
            _isLoading = false;
        }

        /// <summary>
        /// ??ʂ?t?F?[?h?C??????
        /// </summary>
        private async Awaitable FadeIn(float time) =>
            await ServiceLocator.GetInstance<GameLogic>().FadeIn(time);

        /// <summary>
        /// ??ʂ?t?F?[?h?A?E?g????
        /// </summary>
        private async Awaitable FadeOut(float time) =>
            await ServiceLocator.GetInstance<GameLogic>().FadeOut(time);


        /// <summary>
        /// ???[?h?Ɏ??s?????ꍇ?̓z?[???ɖ߂?
        /// </summary>
        private async void LoadFailed()
        {
            await FadeOut(1);
            await SceneLoader.LoadScene(SceneEnum.Home.ToString());
            await FadeIn(1);
        }
    }

    public enum SceneEnum
    {
        Home,
        InGame,
        LoadingScene,
    }
}
