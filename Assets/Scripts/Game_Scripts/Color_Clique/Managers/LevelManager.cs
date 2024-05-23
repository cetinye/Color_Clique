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
        [SerializeField] private int numberOfSlots;
        [SerializeField] private int rotationSpeed;

        [Header("Scene Components")]
        [SerializeField] PickerWheel wheel;
        [SerializeField] Image winnerImg;
        [SerializeField] private Animator crowdAnimator;
        [SerializeField] private Animator curtainAnimator;

        void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            AssignLevelVariables();
            AssignWheelVariables();
            wheel.Initialize();

            OpenCurtains();
        }

        private void AssignLevelVariables()
        {
            levelSO = levels[levelId];

            numberOfSlots = levelSO.slotCount;
            rotationSpeed = levelSO.rotationSpeed;
        }

        private void AssignWheelVariables()
        {
            wheel.AssignWheelVariables(numberOfSlots, rotationSpeed);
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
            winnerImg.sprite = winner;
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