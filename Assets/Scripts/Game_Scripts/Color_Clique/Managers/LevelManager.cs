using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Color_Clique
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Scene Variables")]
        [SerializeField] private int numberOfSlots;

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
            OpenCurtains();
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