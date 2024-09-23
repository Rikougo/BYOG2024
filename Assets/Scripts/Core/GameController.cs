using System.Collections;
using System.Linq;
using Cinemachine;
using TrashBoat.Core.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrashBoat.Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameManager m_gameManager;
        [SerializeField] private TeamController m_teamController;
        [SerializeField] private BossController m_bossController;
        [SerializeField] private BossAnimationsHandler m_bossAnimationsHandler;
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private Button m_leaveButton;
        [SerializeField] private CinemachineVirtualCamera m_virtualCamera;

        private float m_currency;

        public float Currency => m_currency;
        
        private void Start()
        {
            m_currency = 0.0f;
            
            m_teamController.InitRoaster(AttackType.SHIELD, 
                                         AttackType.DRILL, 
                                         AttackType.FLAME,
                                         AttackType.HEAL); 
            m_bossController.Init();

            m_bossController.OnDamageReceived += this.OnBossDamaged;
            m_bossController.OnDefeated += this.OnBossDefeated;

            m_bossAnimationsHandler.OnAnimationHit += this.OnBossAnimationHit;
            
            m_teamController.OnUnitDie += this.OnUnitDie;

            m_resumeButton.gameObject.SetActive(false);
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

        private void OnBossAnimationHit(AttackType p_type)
        {
            if (m_gameManager.CurrentState == GameState.GAME)
            {
                m_bossController.AttackHit(p_type, m_teamController);
                this.StartCoroutine(_ProcessShake());
            }
        }
        
        private IEnumerator _ProcessShake(float p_shakeIntensity = 5f, float p_shakeTiming = 0.5f)
        {
                this.Noise(2.5f, p_shakeIntensity);
                yield return new WaitForSeconds(p_shakeTiming);
                this.Noise(0, 0);
        }

        private void Noise(float p_amplitudeGain, float p_frequencyGain)
        {
            m_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = p_amplitudeGain;
            m_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = p_frequencyGain;
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
            m_resumeButton.gameObject.SetActive(true);

            m_teamController.Pause();
        }

        private void ResumeGame()
        {
            m_gameManager.CurrentState = GameState.GAME;
            
            m_teamController.Reset();
            m_bossController.Reset();
            m_resumeButton.gameObject.SetActive(false);
        }

        private void EndGame()
        {
            m_gameManager.CurrentState = GameState.GAME_OVER;
            m_leaveButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
            m_leaveButton.gameObject.SetActive(true);
            m_resumeButton.gameObject.SetActive(false);
        }
    }
}