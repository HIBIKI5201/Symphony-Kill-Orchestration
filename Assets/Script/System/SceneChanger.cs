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

            //���[�h�V�[���̃��[�h
            if (await SceneLoader.LoadScene(SceneEnum.LoadingScene.ToString()))
            {
                Debug.LogError("���[�h�V�[����ǂݍ��߂܂���ł���");
                return;
            }

            //���̃V�[�����A�����[�h
            bool unloadSuccess = await SceneLoader.UnloadScene(_currentScene.ToString(),
                progress =>
                {
                    ProgressBarUpdate(progress / 2); //0�`50%�͈̔�
                });


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
                    //���[�h�ɐ��������猻�݂̃V�[����Enum���L������
                    _currentScene = sceneEnum;
                }
                else
                {
                    //���[�h�Ɏ��s�����ꍇ�̓z�[���ɋA��
                    await SceneLoader.LoadScene(SceneEnum.Home.ToString());
                }
            }
            else return;

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
