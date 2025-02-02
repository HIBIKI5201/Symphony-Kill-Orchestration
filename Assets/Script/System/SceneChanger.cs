using SymphonyFrameWork.CoreSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Orchestration.System
{
    public class SceneChanger
    {
        private string _currentSceneName;

        public SceneChanger()
        {
            Scene scene = SceneManager.GetActiveScene();
            _currentSceneName = scene.name;
        }

        public async void SceneLoad(SceneEnum sceneEnum)
        {
            FadeOut();

            //ロードシーンのロード
            if (!await SceneLoader.LoadScene(SceneEnum.LoadingScene.ToString()))
            {
                Debug.LogError("ロードシーンを読み込めませんでした");
                return;
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

            FadeIn();

            //元のシーンをアンロード
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentSceneName.ToString(),
                progress =>
                {
                    manager.ProgressBarUpdate(progress / 2); //0～50%の範囲
                });

            await Awaitable.WaitForSecondsAsync(1);

            if (unloadSuccess)
            {
                //シーンをロード
                bool loadSuccess = await SceneLoader.LoadScene(sceneEnum.ToString(),
                    progress =>
                    {
                        manager.ProgressBarUpdate(progress / 2 + 0.5f); //50～100%の範囲

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

            FadeOut();

            //ロードシーンをアンロード
            await SceneLoader.UnloadScene(SceneEnum.LoadingScene.ToString());

            FadeIn();
        }

        /// <summary>
        /// 画面をロードしたシーンの画面にする
        /// </summary>
        private void FadeIn()
        {

        }

        /// <summary>
        /// 画面をロードシーンに移行する
        /// </summary>
        private void FadeOut()
        {

        }


        /// <summary>
        /// ロードに失敗した場合はホームに戻る
        /// </summary>
        private async void LoadFailed()
        {
            FadeOut();
            await SceneLoader.LoadScene(SceneEnum.Home.ToString());
            FadeIn();
        }
    }

    public enum SceneEnum
    {
        Home,
        InGame,
        LoadingScene,
    }
}
