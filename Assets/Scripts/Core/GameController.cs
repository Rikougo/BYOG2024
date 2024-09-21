using System.Linq;
using TrashBoat.Core.Units;
using UnityEngine;
using UnityEngine.UI;

namespace TrashBoat.Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameManager m_gameManager;
        [SerializeField] private TeamController m_teamController;
        [SerializeField] private BossController m_bossController;
        [SerializeField] private Button m_resumeButton;

        private float m_currency;

        public float Currency => m_currency;
        
        private void Start()
        {
            m_currency = 0.0f;
            
            m_teamController.InitRoaster(AttackType.SHIELD, 
                                         AttackType.DRILL, 
                                         AttackType.HEAL, 
                                         AttackType.FLAME);
            m_bossController.Init();

            m_bossController.OnDamageReceived += this.OnBossDamaged;
            m_bossController.OnDefeated += this.OnBossDefeated;
            
            m_teamController.OnUnitDie += this.OnUnitDie;

            m_resumeButton.interactable = false;
            m_resumeButton.onClick.AddListener(this.ResumeGame);

            m_gameManager.CurrentState = GameState.GAME;
        }
        
        private void Update()
        {
            if (m_gameManager.CurrentState == GameState.GAME)
            {
                m_bossController.Tick(m_teamController);
                m_teamController.Tick(m_bossController);
            }
        }

        private void OnBossDamaged(float p_amount, bool p_isArmor, AttackType p_type)
        {
            if (!p_isArmor)
            {
                m_currency += p_amount;
            }
        }

        private void OnBossDefeated()
        {
            this.PauseGame();
        }

        private void OnUnitDie(AttackType p_type)
        {
            if (!m_teamController.UnitSlots.Any(p_slot => p_slot.isActive))
            {
                this.EndGame();
            }
            
            m_bossController.OnUnitDeath(p_type);
        }

        private void PauseGame()
        {
            m_gameManager.CurrentState = GameState.SELECTION;
            // Time.timeScale = 0;
            m_resumeButton.interactable = true;

            m_teamController.Pause();
        }

        private void ResumeGame()
        {
            m_gameManager.CurrentState = GameState.GAME;
            // Time.timeScale = 1;
            
            m_teamController.Reset();
            m_bossController.Reset();
            m_resumeButton.interactable = false;
        }

        private void EndGame()
        {
            m_gameManager.CurrentState = GameState.GAME_OVER;
            Time.timeScale = 0;
        }
    }
}