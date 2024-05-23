using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Color_Clique
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Variables")]
        [SerializeField] private int levelId;
        [SerializeField] private LevelSO levelSO;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();

        [Header("Scene Variables")]
        private int numberOfSlots;
        private int rotationSpeed;
        private bool isTimerOn;
        private float levelTimer;

        [Header("Scene Components")]
        [SerializeField] UIManager uiManager;
        [SerializeField] PickerWheel wheel;
        [SerializeField] Image clickedImg;
        [SerializeField] private Animator crowdAnimator;
        [SerializeField] private Animator curtainAnimator;

        [Header("Flash Interval")]
        [SerializeField] private bool isFlashable = true;

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
            levelTimer = levelSO.time;
        }

        private void AssignWheelVariables()
        {
            wheel.AssignWheelVariables(numberOfSlots, rotationSpeed);
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