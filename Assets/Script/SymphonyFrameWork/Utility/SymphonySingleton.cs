using SymphonyFrameWork.CoreSystem;
using System;
using System.Reflection;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    public class SymphonySingleton : MonoBehaviour
    {
        [SerializeField]
        Component _target;

        private void OnEnable()
        {
            if (_target)
            {
                //Targetのクラスをキャストして実行する
                Type targetType = _target.GetType();
                MethodInfo method = typeof(ServiceLocator).GetMethod(nameof(ServiceLocator.SetInstance))
                    .MakeGenericMethod(targetType);

                method.Invoke(null, new object[] { _target, ServiceLocator.LocateType.Locator });
            }
        }

        private void Start()
        {
            var a = ServiceLocator.GetInstance<Camera>();
            Debug.Log(a.name);
        }

        private void OnDisable()
        {
            if (_target)
            {
                //Targetのクラスをキャストして実行する
                Type targetType = _target.GetType();
                MethodInfo method = typeof(ServiceLocator)
                    .GetMethod(nameof(ServiceLocator.DestroyInstance),
                    BindingFlags.Static | BindingFlags.Public, null,
                    new Type[] { typeof(Component) }, null);

                method.Invoke(null, new object[] { _target });
            }
        }
    }
}