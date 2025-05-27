using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using SymphonyFrameWork.CoreSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    public class StaticFieldEditorUI : EditorWindow
    {
        private FieldInfo _pauseField;
        private VisualElement _pauseVisual;

        private FieldInfo sceneDictField;
        private ListView listView;
        private Dictionary<string, Scene> sceneDict;

        private static VisualElement ElementBase
        {
            get
            {
                var element = new VisualElement()
                {
                    style =
                    {
                        alignItems = Align.Center,
                        alignSelf = Align.Center,
                        alignContent = Align.Center,
                        
                        top = 20,
                        bottom = 20,
                    }
                };
                return element;
            }
        }

        private static Label Title
        {
            get
            {
                var element = new Label()
                {
                    style =
                    {
                        fontSize = 20,
                        bottom = 10,
                    }
                };
                return element;
            }
        }

        [MenuItem("Symphony FrameWork/Admin")]
        public static void ShowWindow()
        {
            StaticFieldEditorUI wnd = GetWindow<StaticFieldEditorUI>();
            wnd.titleContent = new GUIContent("Static Field Editor UI");
        }

        private void OnEnable()
        {
            // `_pause` フィールドを取得
            _pauseField = typeof(PauseManager).GetField("_pause", BindingFlags.Static | BindingFlags.NonPublic);

            // UI を作成
            var root = rootVisualElement;
            PauseInit(root);
            SceneDictInit(root);

            EditorApplication.update += PauseVisualUpdate;
            EditorApplication.update += UpdateList;
        }

        private void OnDisable()
        {
            EditorApplication.update -= PauseVisualUpdate;
            EditorApplication.update -= UpdateList;
        }

        private void PauseInit(VisualElement root)
        {
            VisualElement @base = ElementBase;

            // ラベル
            Label pauseTitle = Title;
            pauseTitle.text = "Pause 状態";

            @base.Add(pauseTitle);

            // Toggle (チェックボックス)
            _pauseVisual = new VisualElement()
            {
                style =
                {
                    width = 40,
                    height = 40,
                }
            };

            // 初期値を設定


            @base.Add(_pauseVisual);

            root.Add(@base);
        }

        private void SceneDictInit(VisualElement root)
        {
            VisualElement @base = ElementBase;

            Label pauseTitle = Title;
            pauseTitle.text = "登録されているシーン";

            @base.Add(pauseTitle);

            sceneDictField = typeof(PauseManager).GetField("_sceneDict", BindingFlags.Static | BindingFlags.NonPublic);

            listView = new ListView
            {
                makeItem = () => new Label(),
                bindItem = (element, index) =>
                {
                    var kvp = GetSceneList()[index];
                    (element as Label).text = $"{kvp.Key} -> {kvp.Value.name}";
                },
                itemsSource = GetSceneList(),
                selectionType = SelectionType.None
            };

            root.Add(listView);
        }

        private List<KeyValuePair<string, Scene>> GetSceneList()
        {
            if (sceneDictField != null)
            {
                sceneDict = (Dictionary<string, Scene>)sceneDictField.GetValue(null);
            }
            return sceneDict != null ? new List<KeyValuePair<string, Scene>>(sceneDict) : new List<KeyValuePair<string, Scene>>();
        }

        private void PauseVisualUpdate()
        {
            if (_pauseField != null)
            {
                bool active = (bool)_pauseField.GetValue(null);
                _pauseVisual.style.backgroundColor = active ? Color.green : Color.red;
            }
            else
            {
                _pauseVisual.style.backgroundColor = Color.red;
            }
        }
        private void UpdateList()
        {
            listView.itemsSource = GetSceneList();
            listView.Rebuild();
        }
    }
}
#endif