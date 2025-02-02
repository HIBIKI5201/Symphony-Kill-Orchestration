using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Utility
{
    public abstract class SymphonyVisualElement : VisualElement
    {
        public Task InitializeTask { get; private set; }

        public SymphonyVisualElement(string path)
        {
            InitializeTask = Initialize(path);
        }

        private async Task Initialize(string path)
        {
            VisualTreeAsset treeAsset = default;
            if (!string.IsNullOrEmpty(path))
            {
                treeAsset = Resources.Load<VisualTreeAsset>(path);
            }
            else
            {
                Debug.LogError($"{name} failed initialize");
                return;
            }


            if (treeAsset != null)
            {
                #region �e�G�������g�̏�����

                var container = treeAsset.Instantiate();
                container.style.width = Length.Percent(100);
                container.style.height = Length.Percent(100);

                this.RegisterCallback<KeyDownEvent>(e => e.StopPropagation());
                pickingMode = PickingMode.Ignore;
                container.RegisterCallback<KeyDownEvent>(e => e.StopPropagation());
                container.pickingMode = PickingMode.Ignore;

                hierarchy.Add(container);

                #endregion

                // UI�v�f�̎擾
                await Initialize_S(container);
                Debug.Log("�E�B���h�E�͐���Ƀ��[�h����");
            }
            else
            {
                Debug.LogError($"Failed to load UXML file from Addressables: {path}");
            }
        }

        protected abstract Task Initialize_S(TemplateContainer container);
    }
}
