using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Color_Clique
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] TMP_Text levelTimeText;

        [Header("Flash Variables")]
        [SerializeField] private float flashInterval = 0.5f;
        private Color defaultColor;

        void Awake()
        {
            defaultColor = levelTimeText.color;
        }

        public void SetTimeText(float time)
        {
            levelTimeText.text = time.ToString("F0");
        }

        public void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(levelTimeText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(levelTimeText.DOColor(defaultColor, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }
    }
}