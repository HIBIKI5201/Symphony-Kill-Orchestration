using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class SceneChanger
    {
        private SceneEnum _currentScene;

        public async void SceneLoad(SceneEnum sceneEnum)
        {
            FadeOut();

            //ロードシーンのロード
            if (await SceneLoader.LoadScene(SceneEnum.LoadingScene.ToString()))
            {
                Debug.LogError("ロードシーンを読み込めませんでした");
                return;
            }

            //元のシーンをアンロード
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentScene.ToString(),
                progress =>
                {
                    ProgressBarUpdate(progress / 2); //0～50%の範囲
                });


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
                    //ロードに成功したら現在のシーンのEnumを記憶する
                    _currentScene = sceneEnum;
                }
                else
                {
                    //ロードに失敗した場合はホームに帰る
                    await SceneLoader.LoadScene(SceneEnum.Home.ToString());
                }
            }
            else return;

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
