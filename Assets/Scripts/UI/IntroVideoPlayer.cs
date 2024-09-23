using UnityEngine;
using UnityEngine.Video;

namespace TrashBoat.UI
{
    public class IntroVideoPlayer : MonoBehaviour
    {
        [SerializeField] private Canvas m_ui;
        [SerializeField] private VideoPlayer m_player;

        private bool m_videoEnded;

        private void Start()
        {
            if (PlayerPrefs.GetInt("SkipIntro", 0) == 0)
            {
                m_ui.gameObject.SetActive(false);
                m_player.Play();
                m_player.loopPointReached += this.OnVideoEnd;
            }
            else
            {
                m_ui.gameObject.SetActive(true);
                Destroy(m_player.gameObject);
            }
        }

        private void OnVideoEnd(VideoPlayer p_player)
        {
            PlayerPrefs.SetInt("SkipIntro", 1);
            m_ui.gameObject.SetActive(true);
            Destroy(m_player.gameObject);
        }
    }
}