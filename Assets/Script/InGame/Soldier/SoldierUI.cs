using Orchestration.InGame;
using Orchestration.UI;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Orchestration.Entity
{
    public class SoldierUI : MonoBehaviour
    {
        private UnitInfomationSoldier _info;
        private UnitSelectorSoldier _selector;

        private UIDocument _document;
        private VisualElement _soldierMark;

        private void Awake()
        {
            _document = GetComponentInChildren<UIDocument>();
            if (_document)
            {
                _soldierMark = _document.rootVisualElement.Q<VisualElement>("Mark");
            }
        }

        public void MarkColorSet(Color color)
        {
            _soldierMark.style.unityBackgroundImageTintColor = color;
        }

        public void AddInfomationForHUD(string name, Texture2D icon)
        {
            var ingameUIManager = ServiceLocator.GetInstance<IngameUIManager>();
            if (ingameUIManager)
            {
                _info = new UnitInfomationSoldier();
                _info.Init(name);
                ingameUIManager.AddSoldierInfo(_info);

                _selector = new UnitSelectorSoldier();
                _selector.Init(icon);
                ingameUIManager.AddSoldierSelector(_selector);
            }
        }

        public async void Select(bool active)
        {
            if (active)
            {
                _soldierMark.style.unityBackgroundImageTintColor = Color.yellow;
            }
            else
            {
                _soldierMark.style.unityBackgroundImageTintColor = Color.blue;
            }

            try
            {
                await SymphonyTask.WaitUntil(() => _info != null && _selector != null, destroyCancellationToken);

                _info.Selected(active);
                _selector.Selected(active);
            }
            catch { }
        }

        /// <summary>
        /// ヘルスバーの位置を更新する
        /// </summary>
        /// <param name="pos"></param>
        public void MarkMove(Vector3 pos, Vector3 healthBarOffset)
        {
            if (_soldierMark == null)
            {
                return;
            }

            //スクリーン座標系に変換
            Vector2 screenPos = Camera.main.WorldToScreenPoint(pos + healthBarOffset);

            float centerX = screenPos.x - (_soldierMark.resolvedStyle.width / 2);
            float centerY = Screen.height - screenPos.y; //UITK座標系では値が高いほど下に移動する

            _soldierMark.style.left = centerX;
            _soldierMark.style.top = centerY;
        }

        /// <summary>
        /// ダメージ表示を追加
        /// </summary>
        /// <param name="damage"></param>
        public void DamageTextInstantiate(float damage)
        {
            if (_document && _document.rootVisualElement != null)
            {
                DamageText text = new DamageText();
                _document.rootVisualElement.Add(text);
                text.Init(damage, transform.position);
            }
        }

        /// <summary>
        /// ヘルスバーの量を更新する
        /// </summary>
        /// <param name="proportion"></param>
        public void HealthBarUpdate(float proportion) => _info.HealthBarUpdate(proportion);

        /// <summary>
        /// スペシャルポイントの量を更新する
        /// </summary>
        /// <param name="proportion"></param>
        /// <param name="count"></param>
        public void SpecialPointGuageUpdate(float proportion) => _selector.SpecialPointGuageUpdate(proportion);

        /// <summary>
        /// スペシャルポイントのカウント量を更新する
        /// </summary>
        /// <param name="count"></param>
        public void SpecialPointCountUpdate(int count) => _selector.SpecialPointCountUpdate(count);

        public void MoveLineRender(NavMeshAgent agent, LineRenderer renderer)
        {
            if (agent)
            {
                Vector3[] corners = agent.path.corners;
                renderer.positionCount = corners.Length;
                renderer.SetPositions(corners);
            }
        }

        private void OnDestroy()
        {
            _info?.Destroy();
            _info = null;

            _selector?.Destroy();
            _selector = null;
        }
    }
}
