using SymphonyFrameWork.CoreSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Orchestration
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

            Scene? scene = SceneLoader.GetExistScene(SceneEnum.LoadingScene.ToString());

            FadeIn();

            //元のシーンをアンロード
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentSceneName.ToString(),
                progress =>
                {
                    ProgressBarUpdate(progress / 2); //0～50%の範囲
                });

            await Awaitable.WaitForSecondsAsync(5);

            if (unloadSuccess)
            {
                //シーンをロード
                bool loadSuccess = await SceneLoader.LoadScene(sceneEnum.ToString(),
                    progress =>
                    {
                        ProgressBarUpdate(progress / 2 + 0.5f); //50～100%の範囲

                    });

                if (loadSuccess)
                {
                    //ロードに成功したら現在のシーンをアクティブ化して記憶する
                    Scene? scene = SceneLoader.GetExistScene(sceneEnum.ToString());
                    if (scene.HasValue)
                    {
                        SceneManager.SetActiveScene(scene.Value);
                        _currentSceneName = sceneEnum.ToString();
                    }
                }
                else
                {
                    //ロードに失敗した場合はホームに戻る
                    await SceneLoader.LoadScene(SceneEnum.Home.ToString());
                }
            }
            else return;

            FadeOut();

            //ロードシーンをアンロード
            await SceneLoader.UnloadScene(SceneEnum.LoadingScene.ToString());

            FadeIn();
        }

        private void ProgressBarUpdate(float progress)
        {

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
    }

    public enum SceneEnum
    {
        Home,
        InGame,
        LoadingScene,
    }
}
