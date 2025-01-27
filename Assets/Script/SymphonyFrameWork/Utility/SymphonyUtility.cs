using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    public static class SymphonyUtility
    {
        public static bool NullCheckComponent(this Component component, string message)
        {
            if (component)
            {
                return true;
            }

            Debug.LogError(message);
            return false;
        }
    }
}
