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

            //���[�h�V�[���̃��[�h
            if (!await SceneLoader.LoadScene(SceneEnum.LoadingScene.ToString()))
            {
                Debug.LogError("���[�h�V�[����ǂݍ��߂܂���ł���");
                return;
            }

            Scene? scene = SceneLoader.GetExistScene(SceneEnum.LoadingScene.ToString());

            FadeIn();

            //���̃V�[�����A�����[�h
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentSceneName.ToString(),
                progress =>
                {
                    ProgressBarUpdate(progress / 2); //0�`50%�͈̔�
                });

            await Awaitable.WaitForSecondsAsync(5);

            if (unloadSuccess)
            {
                //�V�[�������[�h
                bool loadSuccess = await SceneLoader.LoadScene(sceneEnum.ToString(),
                    progress =>
                    {
                        ProgressBarUpdate(progress / 2 + 0.5f); //50�`100%�͈̔�

                    });

                if (loadSuccess)
                {
                    //���[�h�ɐ��������猻�݂̃V�[�����A�N�e�B�u�����ċL������
                    Scene? scene = SceneLoader.GetExistScene(sceneEnum.ToString());
                    if (scene.HasValue)
                    {
                        SceneManager.SetActiveScene(scene.Value);
                        _currentSceneName = sceneEnum.ToString();
                    }
                }
                else
                {
                    //���[�h�Ɏ��s�����ꍇ�̓z�[���ɖ߂�
                    await SceneLoader.LoadScene(SceneEnum.Home.ToString());
                }
            }
            else return;

            FadeOut();

            //���[�h�V�[�����A�����[�h
            await SceneLoader.UnloadScene(SceneEnum.LoadingScene.ToString());

            FadeIn();
        }

        private void ProgressBarUpdate(float progress)
        {

        }

        /// <summary>
        /// ��ʂ����[�h�����V�[���̉�ʂɂ���
        /// </summary>
        private void FadeIn()
        {

        }

        /// <summary>
        /// ��ʂ����[�h�V�[���Ɉڍs����
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
