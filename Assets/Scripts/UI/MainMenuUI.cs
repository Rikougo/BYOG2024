using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrashBoat.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameManager m_gameManager;
        
        [Header("Main menu buttons")] 
        [SerializeField] private Transform m_mainMenuLayout;
        [SerializeField] private Button m_startButton;
        [SerializeField] private Button m_creditsButton;
        [SerializeField] private Button m_exitButton;

        [Header("Credits")] 
        [SerializeField] private Transform m_creditsLayout;
        [SerializeField] private Button m_creditsBackButton;

        private void Start()
        {
            m_mainMenuLayout.gameObject.SetActive(true);
            m_creditsLayout.gameObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            m_startButton.onClick.AddListener(this.OnStartGame);
            m_creditsButton.onClick.AddListener(() => this.ToggleCredits(true));
            m_exitButton.onClick.AddListener(this.OnExitGame);
        }

        private void OnDisable()
        {
            m_startButton.onClick.RemoveAllListeners();
            m_creditsButton.onClick.RemoveAllListeners();
            m_exitButton.onClick.RemoveAllListeners();
        }

        private void OnStartGame()
        {
            m_gameManager.CurrentState = GameState.GAME;
            SceneManager.LoadScene("MainScene");
        }

        private void OnExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ToggleCredits(bool p_value)
        {
            m_creditsLayout.gameObject.SetActive(p_value);
            m_mainMenuLayout.gameObject.SetActive(!p_value);

            if (p_value)
            {
                m_creditsBackButton.onClick.AddListener(() => this.ToggleCredits(false));
            }
            else
            {
                m_creditsBackButton.onClick.RemoveAllListeners();
            }
        }
    } 
}
