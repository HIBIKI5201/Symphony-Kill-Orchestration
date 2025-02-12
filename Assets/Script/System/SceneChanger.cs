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

        public SceneChanger()
        {
            Scene scene = SceneManager.GetActiveScene();
            _currentSceneName = scene.name;
        }

        public async void SceneLoad(SceneEnum sceneEnum)
        {
            if (_isLoading)
            {
                Debug.LogWarning("ローディング中にロードできません");
                return;
            }
            _isLoading = true;

            await FadeOut(1.5f);

            //ロードシーンのロード
            if (!await SceneLoader.LoadScene(SceneEnum.LoadingScene.ToString()))
            {
                Debug.LogError("ロードシーンを読み込めませんでした");
                return;
            }
            else
            {
                //ロードシーンをアクティブに
                SceneLoader.GetExistScene(SceneEnum.LoadingScene.ToString(), out Scene scene);
                SceneManager.SetActiveScene(scene);
            }

            //ロードシーンのマネージャーを取得する
            LoadSceneManager manager = null;

            if (SceneLoader.GetExistScene(SceneEnum.LoadingScene.ToString(), out Scene loadScene))
            {
                //ロードシーンのルートオブジェクトからロードシーンマネージャーを探す
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
                    Debug.LogError("ロードシーンマネージャーが見つかりません");
                    LoadFailed();
                    return;
                }
            }

            await FadeIn(1);

            //元のシーンをアンロード
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentSceneName.ToString(),
                progress =>
                {
                    manager.ProgressUpdate(progress / 2); //0～50%の範囲
                });

            //少し猶予を作る
            await Awaitable.WaitForSecondsAsync(0.5f);

            if (unloadSuccess)
            {
                //シーンをロード
                bool loadSuccess = await SceneLoader.LoadScene(sceneEnum.ToString(),
                    progress =>
                    {
                        manager.ProgressUpdate(progress / 2 + 0.5f); //50～100%の範囲
                    });

                if (loadSuccess)
                {
                    //ロードに成功したら現在のシーンをアクティブ化して記憶する
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

            //完了した事を明示するためにゲージを最大化
            manager.ProgressUpdate(1);

            //少し猶予を作る
            await Awaitable.WaitForSecondsAsync(0.5f);

            await FadeOut(1);

            //ロードシーンをアンロード
            await SceneLoader.UnloadScene(SceneEnum.LoadingScene.ToString());

            await FadeIn(1.5f);

            _isLoading = false;
        }

        /// <summary>
        /// 画面をフェードインする
        /// </summary>
        private async Awaitable FadeIn(float time) =>
            await ServiceLocator.GetInstance<GameLogic>().FadeIn(time);

        /// <summary>
        /// 画面をフェードアウトする
        /// </summary>
        private async Awaitable FadeOut(float time) =>
            await ServiceLocator.GetInstance<GameLogic>().FadeOut(time);


        /// <summary>
        /// ロードに失敗した場合はホームに戻る
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
