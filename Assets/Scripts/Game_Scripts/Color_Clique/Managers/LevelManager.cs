using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Color_Clique
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [Header("Level Variables")]
        [SerializeField] private int levelId;
        [SerializeField] private LevelSO levelSO;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();

        [Header("Scene Variables")]
        private int numberOfSlots;
        private int rotationSpeed;
        private int needleRotateSpeed;
        private bool isTimerOn;
        private float levelTimer;
        private int correctCount = 0;
        private int wrongCount = 0;
        private int comboCounter = 0;

        [Header("Scene Components")]
        [SerializeField] UIManager uiManager;
        [SerializeField] PickerWheel wheel;
        [SerializeField] Image clickedImg;
        [SerializeField] Image selectedImg;
        [SerializeField] private Animator crowdAnimator;
        [SerializeField] private Animator curtainAnimator;
        [SerializeField] private ParticleSystem combo;
        [SerializeField] private TMPro.TMP_Text comboText;

        [Header("Flash Interval")]
        [SerializeField] private bool isFlashable = true;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            StartGame();
        }

        void Update()
        {
            LevelTimer();
        }

        private void StartGame()
        {
            AssignLevelVariables();
            AssignWheelVariables();
            wheel.Initialize();

            OpenCurtains();
            isTimerOn = true;
        }

        private void AssignLevelVariables()
        {
            levelSO = levels[levelId];

            numberOfSlots = levelSO.slotCount;
            rotationSpeed = levelSO.rotationSpeed;
            needleRotateSpeed = levelSO.needleRotateSpeed;
            levelTimer = levelSO.time;
        }

        private void AssignWheelVariables()
        {
            wheel.AssignWheelVariables(numberOfSlots, rotationSpeed, needleRotateSpeed);
        }

        private void LevelTimer()
        {
            if (!isTimerOn) return;

            levelTimer -= Time.deltaTime;

            if (levelTimer < 0)
            {
                isTimerOn = false;
                levelTimer = 0;
            }

            if (levelTimer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                uiManager.FlashRed();
            }

            uiManager.SetTimeText(levelTimer);
        }

        public void Check(Sprite clickedImage)
        {
            if (selectedImg.sprite == clickedImage)
            {
                correctCount++;
                comboCounter++;
                uiManager.UpdateStats(correctCount, wrongCount);

                if (comboCounter >= 2)
                {
                    PlayCombo();
                }
            }
            else
            {
                wrongCount++;
                comboCounter = 0;
                uiManager.UpdateStats(correctCount, wrongCount);
            }
        }

        public void PlayCombo()
        {
            comboText.text = comboCounter.ToString() + "x Combo";
            combo.Play();
        }

        public void SpinWheel()
        {
            wheel.Spin();
            wheel.OnSpinEnd(wheelPiece =>
            {
                Debug.Log(wheelPiece.Label);
            });
        }

        public void StopWheel()
        {
            //wheel.Stop();

            Sprite winner = wheel.GetImage();
            clickedImg.sprite = winner;
            Check(clickedImg.sprite);
        }

        public void SetSelectedItem(Sprite sprite)
        {
            selectedImg.sprite = sprite;
        }

        #region Animations

        public void OpenCurtains()
        {
            curtainAnimator.Play("CurtainOpen");
        }

        public void CrowdClap()
        {
            crowdAnimator.Play("Clap");
        }

        public void CrowdShout()
        {
            crowdAnimator.Play("Shout");
        }

        #endregion
    }
}