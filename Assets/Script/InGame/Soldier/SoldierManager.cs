using Orchestration.InGame;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Orchestration.Entity
{
    /// <summary>
    /// ���m�̃x�[�X�N���X
    /// </summary>
    [RequireComponent(typeof(SoldierMove), typeof(SoldierUI))]
    public class SoldierManager : MonoBehaviour
    {
        [SerializeField]
        private SoldierData_SO _soldierData;

        private SoldierMove _moveModule;

        private SoldierUI _ui;

        private void Awake()
        {
            var data = Instantiate(_soldierData);
            _soldierData = data;

            _moveModule = GetComponent<SoldierMove>();

            _ui = GetComponent<SoldierUI>();

            if (_moveModule.NullCheckComponent($"{name}�̃��f����������܂���ł���") && _ui.NullCheckComponent($"{name}��UI��������܂���ł���"))
            {
                _soldierData.OnHealthChanged += value => _ui.HealthBarUpdate(value / data.MaxHealthPoint);
                _soldierData.OnHealthChanged += value => Debug.Log(value);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _moveModule.SetDirection();
            }

            GridHighLight();

            _moveModule.Move();

            _ui.HealthBarMove(transform.position);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ServiceLocator.GetSingleton<GameLogic>().SceneChange(System.SceneEnum.InGame);
            }
        }

        private void GridHighLight()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridManager = ServiceLocator.GetSingleton<GridManager>();

                //�q�b�g�����ꏊ�̃O���b�h�ʒu��ڕW�n�_�ɃZ�b�g
                if (gridManager.GetGridPosition(hit.point, out Vector3 pos, out int index))
                {
                    gridManager.HighLightGrid(index);
                }
            }
        }

        private void AddDamage(float damage) => _soldierData.HealthPoint -= damage;
        private void AddHeal(float heal) => _soldierData.HealthPoint += heal;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            NavMeshAgent agent = _moveModule?.Agent;

            if (agent != null && agent.path != null)
            {
                NavMeshPath path = agent.path;

                if (path.corners.Length < 2)
                {
                    return;
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawLineStrip(path.corners, false);
            }
        }

        #region ������OnGUI
        /*
        GUIStyle Style
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style = new GUIStyle();
                style.fontSize = 30;
                style.normal.textColor = Color.white;
                return style;
            }
        }

        /// <summary>
        /// ������OnGUI���g�p���Ă݂�
        /// </summary>
        private void OnGUI()
        {
            string[] logs = new string[2] { "a", "b" };

            float y = 10;
            foreach (string log in logs)
            {
                GUI.Label(new Rect(0, y, 350, 40), log, Style);
                y += 40;
            }
        }
        */
        #endregion
#endif
    }
}