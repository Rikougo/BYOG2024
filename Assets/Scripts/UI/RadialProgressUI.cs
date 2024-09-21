using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivivor.UI
{
    public class RadialProgressUI : MonoBehaviour
    {
        [SerializeField] private Image m_fill;

        public void SetProgress(float p_value)
        {
            m_fill.fillAmount = p_value;
        }
    }
}
