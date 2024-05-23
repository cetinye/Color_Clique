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
        private float timer;

        [Header("Scene Components")]
        [SerializeField] UIManager uiManager;
        [SerializeField] PickerWheel wheel;
        [SerializeField] Image clickedImg;
        [SerializeField] private Animator crowdAnimator;
        [SerializeField] private Animator curtainAnimator;

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
            timer = levelSO.time;
        }

        private void AssignWheelVariables()
        {
            wheel.AssignWheelVariables(numberOfSlots, rotationSpeed);
        }

        private void LevelTimer()
        {
            if (!isTimerOn) return;

            timer -= Time.deltaTime;

            if (timer < 0)
            {
                isTimerOn = false;
                timer = 0;
            }

            uiManager.SetTimeText(timer);

            //TODO: Flash Warning, NoInputWarning
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