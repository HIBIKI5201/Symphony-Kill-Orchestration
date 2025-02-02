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

            //���[�h�V�[���̃��[�h
            if (!await SceneLoader.LoadScene(SceneEnum.LoadingScene.ToString()))
            {
                Debug.LogError("���[�h�V�[����ǂݍ��߂܂���ł���");
                return;
            }

            //���[�h�V�[���̃}�l�[�W���[���擾����
            LoadSceneManager manager = null;

            if (SceneLoader.GetExistScene(SceneEnum.LoadingScene.ToString(), out Scene loadScene))
            {
                //���[�h�V�[���̃��[�g�I�u�W�F�N�g���烍�[�h�V�[���}�l�[�W���[��T��
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
                    Debug.LogError("���[�h�V�[���}�l�[�W���[��������܂���");
                    LoadFailed();
                    return;
                }
            }

            FadeIn();

            //���̃V�[�����A�����[�h
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentSceneName.ToString(),
                progress =>
                {
                    manager.ProgressBarUpdate(progress / 2); //0�`50%�͈̔�
                });

            await Awaitable.WaitForSecondsAsync(1);

            if (unloadSuccess)
            {
                //�V�[�������[�h
                bool loadSuccess = await SceneLoader.LoadScene(sceneEnum.ToString(),
                    progress =>
                    {
                        manager.ProgressBarUpdate(progress / 2 + 0.5f); //50�`100%�͈̔�

                    });

                if (loadSuccess)
                {
                    //���[�h�ɐ��������猻�݂̃V�[�����A�N�e�B�u�����ċL������
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

            //���[�h�V�[�����A�����[�h
            await SceneLoader.UnloadScene(SceneEnum.LoadingScene.ToString());

            FadeIn();
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


        /// <summary>
        /// ���[�h�Ɏ��s�����ꍇ�̓z�[���ɖ߂�
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
