using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Color_Clique
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] TMP_Text timeText;

        public void SetTimeText(float time)
        {
            timeText.text = time.ToString("F0");
        }
    }
}