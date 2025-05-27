using System;
using UnityEngine;

namespace SymphonyFrameWork.CoreSystem
{
    public static class SaveDataSystem<DataType> where DataType : new()
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _saveData = null;
        }

        private static SaveData _saveData;
        public static DataType Data
        {
            get
            {
                if (_saveData is null)
                    Load();
                return _saveData.MainData;
            }
        }
        public static string SaveDate
        {
            get
            {
                if (_saveData is null)
                    Load();
                return _saveData.SaveDate;
            }
        }

        public static void Save()
        {
            _saveData = new SaveData(Data);
            string data = JsonUtility.ToJson(_saveData);
            Debug.Log($"{_saveData.SaveDate}\n{data}");
            PlayerPrefs.SetString(typeof(DataType).Name, data);

        }

        private static void Load()
        {
            #region Prefs????f?[?^????[?h????
            string json = PlayerPrefs.GetString(typeof(DataType).Name);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning($"{typeof(DataType).Name}?̃f?[?^????????܂???ł???");
                _saveData = new SaveData(new DataType());
                return;
            }
            #endregion

            #region JSON?ɕϊ????ĕۑ?
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data is not null)
            {
                Debug.Log($"{typeof(DataType).Name}?̃f?[?^?????[?h????܂???\n{data}");
                _saveData = data;
            }
            else
            {
                Debug.LogWarning($"{typeof(DataType).Name}?̃??[?h???o???܂???ł???");
                _saveData = new SaveData(new DataType());
            }
            #endregion
        }

        [Serializable]
        private class SaveData
        {
            public string SaveDate;
            public DataType MainData;

            public SaveData(DataType dataType)
            {
                SaveDate = DateTime.Now.ToString("O");
                MainData = dataType;
            }
        }
    }
}