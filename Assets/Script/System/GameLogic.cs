using SymphonyFrameWork.CoreSystem;
using UnityEngine;

namespace Orchestration
{
    public class GameLogic : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void GameInitialize()
        {
            await SceneLoader.LoadScene("System");
        }


    }
}
