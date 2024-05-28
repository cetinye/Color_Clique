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
        private int numberOfColors;
        private int shapeCount;
        private int wheelSegments;
        private float rotationSpeed;
        private bool isWheelBarReversalEnabled;
        private int minChangeFrequency;
        private int maxChangeFrequency;
        private bool isComboScoreEnabled;
        private int maxScore;
        private float scorePerCorrectOperation;
        private int comboMultiplier;
        private bool isTimerOn;
        private float levelTimer;
        private int correctCount = 0;
        private int wrongCount = 0;
        private int comboCounter = 0;
        private bool isClickable;
        private int moveLimitForChange;
        private int moveCounter;
        private int levelUpCounter;
        private int levelDownCounter;
        private List<int> scores = new List<int>();

        [Header("Scene Components")]
        [SerializeField] UIManager uiManager;
        [SerializeField] Wheel wheel;
        [SerializeField] Image selectedImg;
        [SerializeField] Image selectedImgBG;
        [SerializeField] Color selectedColor;
        [SerializeField] private Animator crowdAnimator;
        [SerializeField] private Animator curtainAnimator;
        [SerializeField] private ParticleSystem combo;
        [SerializeField] private Camera particleCam;
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
            // start from one level down
            levelId--;
            levelId = Mathf.Clamp(levelId, 0, levels.Count - 1);

            AssignLevelVariables();
            AssignWheelVariables();
            SetMoveLimit();
            wheel.Initialize();
            SelectItem();

            OpenCurtains();
            isTimerOn = true;
            isClickable = true;
        }

        private void AssignLevelVariables()
        {
            levelSO = levels[levelId];

            numberOfColors = levelSO.numberOfColors;
            shapeCount = levelSO.shapeCount;
            wheelSegments = levelSO.wheelSegments;
            rotationSpeed = 2 * levelSO.spinSpeedMultiplier;
            isWheelBarReversalEnabled = levelSO.isWheelBarReversalEnabled;
            minChangeFrequency = levelSO.minChangeFrequency;
            maxChangeFrequency = levelSO.maxChangeFrequency;
            levelTimer = levelSO.totalTime;
            isComboScoreEnabled = levelSO.isComboScoreEnabled;
            maxScore = levelSO.maxScore;
            scorePerCorrectOperation = levelSO.scorePerCorrectOperation;
            comboMultiplier = levelSO.comboMultiplier;
        }

        private void AssignWheelVariables()
        {
            wheel.AssignWheelVariables(wheelSegments, rotationSpeed);
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

        private void SetMoveLimit()
        {
            moveLimitForChange = Random.Range(minChangeFrequency, maxChangeFrequency);
            moveCounter = 0;
        }

        public void Check(Sprite clickedImage, Color clickedColor)
        {
            isClickable = false;

            if (selectedImg.sprite == clickedImage && selectedColor == clickedColor)
            {
                correctCount++;
                comboCounter++;
                levelUpCounter++;
                uiManager.UpdateStats(correctCount, wrongCount);
                wheel.SetNeedleColor(Color.green, 0.5f);

                if (comboCounter >= 2)
                {
                    PlayCombo();
                }
            }
            else
            {
                wrongCount++;
                comboCounter = 0;
                levelDownCounter++;
                uiManager.UpdateStats(correctCount, wrongCount);
                wheel.SetNeedleColor(Color.red, 0.5f);
            }

            //select new item if move limit reached
            moveCounter++;
            if (moveCounter >= moveLimitForChange)
            {
                SelectItem();
                SetMoveLimit();
            }

            CheckLevel();
        }

        public void PlayCombo()
        {
            comboText.text = comboCounter.ToString() + "x Combo";
            particleCam.gameObject.SetActive(true);
            combo.Play();
            CancelInvoke(nameof(DisableParticleCam));
            Invoke(nameof(DisableParticleCam), 0.5f);
        }

        private void DisableParticleCam()
        {
            particleCam.gameObject.SetActive(false);
        }

        public void Clicked()
        {
            if (!isClickable) return;

            Slot clickedSlot = wheel.GetClickedSlot();
            Check(clickedSlot.GetItemSprite(), clickedSlot.GetSlotColor());

            if (levelSO.isWheelBarReversalEnabled)
            {
                wheel.ReverseNeedle();
            }
        }

        private void CalculateLevelScore()
        {
            int levelScore = Mathf.CeilToInt(Mathf.Min((correctCount - wrongCount) * scorePerCorrectOperation, maxScore)) + (comboCounter * comboMultiplier);
            levelScore = Mathf.Max(levelScore, 0);
            levelScore = Mathf.Clamp(levelScore, 0, 1000);
            scores.Add(levelScore);
        }

        private int GetTotalScore()
        {
            float total = 0;

            for (int i = 0; i < scores.Count; i++)
            {
                total += scores[i];
            }

            return Mathf.Max(Mathf.CeilToInt(total / scores.Count), 0);
        }

        private void CheckLevel()
        {
            if (levelUpCounter == 3)
            {
                levelUpCounter = 0;
                SetLevel(++levelId);
                CrowdClap();
                CalculateLevelScore();
                uiManager.SetScoreText(GetTotalScore());
            }

            if (levelDownCounter == 2)
            {
                levelDownCounter = 0;
                SetLevel(--levelId);
                CrowdShout();
            }

            uiManager.SetDebugTexts(levelId, levelDownCounter, levelUpCounter);
        }

        private void SetLevel(int levelId)
        {
            this.levelId = Mathf.Clamp(levelId, 0, levels.Count - 1);
            AssignLevelVariables();
            AssignWheelVariables();
            wheel.Initialize();
            SelectItem();
        }

        public void SelectItem()
        {
            Slot selectedSlot = wheel.SelectSlot();

            selectedImg.sprite = selectedSlot.GetItemSprite();
            selectedColor = selectedSlot.GetSlotColor();
            selectedImgBG.color = selectedColor;
        }

        public void SetIsClickable(bool state)
        {
            isClickable = state;
        }

        public Wheel GetWheel()
        {
            return wheel;
        }

        #region Animations

        public void OpenCurtains()
        {
            curtainAnimator.Play("CurtainOpen", -1, 0.0f);
        }

        public void CrowdClap()
        {
            crowdAnimator.Play("Clap", -1, 0.0f);
        }

        public void CrowdShout()
        {
            crowdAnimator.Play("Shout", -1, 0.0f);
        }

        #endregion
    }
}