using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// シングルトンのインスタンスを統括して管理するクラス
    /// </summary>
    //インスタンスを一時的にシーンロードから切り離したい時にも使用できる
    public static class ServiceLocator
    {
        [Tooltip("シングルトン化するインスタンスのコンテナ")]
        private static GameObject _instance;
        [Tooltip("シングルトン登録されている型のインスタンス辞書")]
        private static Dictionary<Type, MonoBehaviour> _singletonObjects = new();

        /// <summary>
        /// インスタンスコンテナが無い場合に生成する
        /// </summary>
        private static void CreateInstance()
        {
            if (_instance is not null) return;

            GameObject instance = new GameObject("SingltonDirector");

            SymphonyCoreSystem.MoveObjectToSymphonySystem(instance);
            _instance = instance;
        }

        public static void SetInstance<T>(T instance, LocateType type) where T : MonoBehaviour
        {

        }

        public enum LocateType
        {
            Singleton,
            Locator,
        }

        #region 旧シングルトン用メソッド
        /// <summary>
        /// 入れられたMonoBehaviourを継承するクラスをシングルトン化する
        /// </summary>
        /// <typeparam name="T">シングルトン化する型</typeparam>
        /// <param name="instance">シングルトンインスタンス</param>
        /// <returns>辞書に追加が成功したらtrue、失敗したらfalse</returns>
        public static void SetSinglton<T>(T instance) where T : MonoBehaviour
        {
            CreateInstance();

            // シングルトンが既に登録されている場合は追加できない
            if (!_singletonObjects.TryAdd(typeof(T), instance))
            {
                Object.Destroy(instance.gameObject);
                return;
            }

            Debug.Log($"{typeof(T).Name}クラスの{instance.name}がシングルトン登録されました");
            instance.transform.SetParent(_instance.transform);
        }

        /// <summary>
        /// 型のクラスがシングルトン化していた場合はインスタンスを返す
        /// </summary>
        /// <typeparam name="T">取得したいシングルトンインスタンスの型</typeparam>
        /// <returns>指定した型のインスタンス</returns>
        public static T GetSingleton<T>() where T : MonoBehaviour
        {
            if (_singletonObjects.TryGetValue(typeof(T), out MonoBehaviour md))
            {
                return md as T;
            }

            Debug.LogError($"{typeof(T).Name} は登録されていません。");
            return null;
        }

        /// <summary>
        /// 指定した型のインスタンスを破棄する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DestroySingleton<T>() where T : MonoBehaviour
        {
            if (_singletonObjects.TryGetValue(typeof(T), out MonoBehaviour md))
            {
                Object.Destroy(md.gameObject);
                _singletonObjects.Remove(typeof(T));
                Debug.Log($"{typeof(T).Name}が破棄されました");
            }
            else
            {
                Debug.Log($"{typeof(T).Name}はシングルトン登録されていません");
            }
        }

#endregion
    }
}