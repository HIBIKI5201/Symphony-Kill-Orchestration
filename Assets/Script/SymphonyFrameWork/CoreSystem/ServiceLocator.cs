using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// �V���O���g���̃C���X�^���X�𓝊����ĊǗ�����N���X
    /// </summary>
    //�C���X�^���X���ꎞ�I�ɃV�[�����[�h����؂藣���������ɂ��g�p�ł���
    public static class ServiceLocator
    {
        [Tooltip("�V���O���g��������C���X�^���X�̃R���e�i")]
        private static GameObject _instance;
        [Tooltip("�V���O���g���o�^����Ă���^�̃C���X�^���X����")]
        private static Dictionary<Type, MonoBehaviour> _singletonObjects = new();

        /// <summary>
        /// �C���X�^���X�R���e�i�������ꍇ�ɐ�������
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

        #region ���V���O���g���p���\�b�h
        /// <summary>
        /// �����ꂽMonoBehaviour���p������N���X���V���O���g��������
        /// </summary>
        /// <typeparam name="T">�V���O���g��������^</typeparam>
        /// <param name="instance">�V���O���g���C���X�^���X</param>
        /// <returns>�����ɒǉ�������������true�A���s������false</returns>
        public static void SetSinglton<T>(T instance) where T : MonoBehaviour
        {
            CreateInstance();

            // �V���O���g�������ɓo�^����Ă���ꍇ�͒ǉ��ł��Ȃ�
            if (!_singletonObjects.TryAdd(typeof(T), instance))
            {
                Object.Destroy(instance.gameObject);
                return;
            }

            Debug.Log($"{typeof(T).Name}�N���X��{instance.name}���V���O���g���o�^����܂���");
            instance.transform.SetParent(_instance.transform);
        }

        /// <summary>
        /// �^�̃N���X���V���O���g�������Ă����ꍇ�̓C���X�^���X��Ԃ�
        /// </summary>
        /// <typeparam name="T">�擾�������V���O���g���C���X�^���X�̌^</typeparam>
        /// <returns>�w�肵���^�̃C���X�^���X</returns>
        public static T GetSingleton<T>() where T : MonoBehaviour
        {
            if (_singletonObjects.TryGetValue(typeof(T), out MonoBehaviour md))
            {
                return md as T;
            }

            Debug.LogError($"{typeof(T).Name} �͓o�^����Ă��܂���B");
            return null;
        }

        /// <summary>
        /// �w�肵���^�̃C���X�^���X��j������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DestroySingleton<T>() where T : MonoBehaviour
        {
            if (_singletonObjects.TryGetValue(typeof(T), out MonoBehaviour md))
            {
                Object.Destroy(md.gameObject);
                _singletonObjects.Remove(typeof(T));
                Debug.Log($"{typeof(T).Name}���j������܂���");
            }
            else
            {
                Debug.Log($"{typeof(T).Name}�̓V���O���g���o�^����Ă��܂���");
            }
        }

#endregion
    }
}