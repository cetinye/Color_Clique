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
        [SerializeField] private float timePerQuestion;
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
        private float scoreToAdd;
        private List<int> scores = new List<int>();

        [Header("Scene Components")]
        [SerializeField] UIManager uiManager;
        [SerializeField] Wheel wheel;
        [SerializeField] SpriteRenderer selectedSp;
        [SerializeField] SpriteRenderer selectedSpBG;
        [SerializeField] Slot selectedSlot;
        [SerializeField] Color selectedColor;
        [SerializeField] private Animator crowdAnimator;
        [SerializeField] private Animator curtainAnimator;
        [SerializeField] private ParticleSystem combo;
        [SerializeField] private List<ParticleSystem> confettis = new List<ParticleSystem>();
        [SerializeField] private Camera particleCam;
        [SerializeField] private TMPro.TMP_Text comboText;

        [Header("Flash Interval")]
        [SerializeField] private bool isFlashable = true;

        void Awake()
        {
            instance = this;

            scores.Clear();
        }

        void Start()
        {
            StartCoroutine(StartGame());
        }

        void Update()
        {
            LevelTimer();
        }

        IEnumerator StartGame()
        {
            // start from one level down
            levelId--;
            levelId = Mathf.Clamp(levelId, 0, levels.Count - 1);

            AssignLevelVariables();
            AssignWheelVariables();
            levelTimer = levelSO.totalTime;
            SetMoveLimit();
            wheel.Initialize();
            SelectItem();

            OpenCurtains();

            yield return new WaitForSeconds(1.1f);

            isTimerOn = true;
            isClickable = true;
            wheel.StartTimer(LevelManager.instance.GetTimePerQuestion());
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
            isComboScoreEnabled = levelSO.isComboScoreEnabled;
            maxScore = levelSO.maxScore;
            scorePerCorrectOperation = levelSO.scorePerCorrectOperation;
            comboMultiplier = levelSO.comboMultiplier;

            scoreToAdd = levelSO.maxScore;
        }

        private void AssignWheelVariables()
        {
            wheel.AssignWheelVariables(wheelSegments, rotationSpeed);
        }

        private void LevelTimer()
        {
            if (!isTimerOn) return;

            levelTimer -= Time.deltaTime;

            if (scoreToAdd > 0)
            {
                scoreToAdd -= Time.deltaTime;
                scoreToAdd = Mathf.Max(scoreToAdd, 0);
            }

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
            uiManager.SetScoreToAdd(scoreToAdd);
        }

        private void SetMoveLimit()
        {
            moveLimitForChange = Random.Range(minChangeFrequency, maxChangeFrequency);
            moveCounter = 0;
        }

        public void Check(Sprite clickedImage, Color clickedColor, bool isTimeOut = false)
        {
            isClickable = false;

            if (!isTimeOut && selectedSp.enabled == true && selectedSp.sprite == clickedImage && selectedColor == clickedColor)
            {
                Correct();
            }
            else if (!isTimeOut && selectedSp.enabled == false && selectedColor == clickedColor)
            {
                Correct();
            }
            else
            {
                Wrong();
            }

            //select new item if move limit reached
            moveCounter++;
            if (moveCounter >= moveLimitForChange)
            {
                SelectItem();
                SetMoveLimit();
            }

            CheckLevel();
            wheel.StartTimer(LevelManager.instance.GetTimePerQuestion());
        }

        private void Correct()
        {
            correctCount++;
            comboCounter++;
            levelUpCounter++;
            uiManager.UpdateStats(correctCount, wrongCount);
            wheel.SetNeedleColor(Color.green, 0.5f);
            SelectItem();

            if (comboCounter >= 2)
            {
                PlayCombo();
            }
            if (comboCounter >= 10)
            {
                PlayConfettis();
            }
        }

        public void Wrong()
        {
            Debug.Log("WRONG");
            wrongCount++;
            comboCounter = 0;
            levelDownCounter++;
            uiManager.UpdateStats(correctCount, wrongCount);
            wheel.SetNeedleColor(Color.red, 0.5f);
        }

        private void PlayCombo()
        {
            comboText.text = comboCounter.ToString() + "x Combo";
            particleCam.gameObject.SetActive(true);
            combo.Play();
            CancelInvoke(nameof(DisableParticleCam));
            Invoke(nameof(DisableParticleCam), 0.5f);
        }

        private void PlayConfettis()
        {
            foreach (ParticleSystem confetti in confettis)
            {
                confetti.Play();
            }
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
            int levelScore = Mathf.CeilToInt(((correctCount - wrongCount) * scorePerCorrectOperation) + (comboCounter * comboMultiplier) + scoreToAdd);
            levelScore = Mathf.Min(levelScore, maxScore);
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

            return Mathf.Clamp(Mathf.Max(Mathf.CeilToInt(total / scores.Count), 0), 0, 1000);
        }

        private void CheckLevel()
        {
            if (levelUpCounter == 3)
            {
                levelUpCounter = 0;
                CalculateLevelScore();
                SetLevel(++levelId);
                CrowdClap();
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
            Slot previousSlot = selectedSlot;

            do
            {
                selectedSlot = wheel.SelectSlot();
                selectedSp.sprite = selectedSlot.GetItemSprite();
                selectedColor = selectedSlot.GetSlotColor();
                selectedSpBG.color = selectedColor;

            } while (previousSlot == selectedSlot);
        }

        public void SetIsClickable(bool state)
        {
            isClickable = state;
        }

        public Wheel GetWheel()
        {
            return wheel;
        }

        public int GetNumberOfColors()
        {
            return numberOfColors;
        }

        public int GetShapeCount()
        {
            return shapeCount;
        }

        public float GetTimePerQuestion()
        {
            return timePerQuestion;
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