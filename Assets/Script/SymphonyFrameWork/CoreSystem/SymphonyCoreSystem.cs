using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.CoreSystem
{
    public static class SymphonyCoreSystem
    {
        private static Scene _systemScene;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EditorModeInitialize()
        {
            Debug.Log("Initialize");

            _systemScene = SceneManager.CreateScene("SymphonySystem");
        }

        public static void MoveObjectToSymphonySystem(GameObject go)
        {
            SceneManager.MoveGameObjectToScene(go, _systemScene);
        }
    }
}
