using UnityEngine;

namespace Color_Clique
{
    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public int numberOfColors;
        public int shapeCount;
        public int wheelSegments;
        public float spinSpeedMultiplier;
        public bool isWheelBarReversalEnabled;
        public int minChangeFrequency;
        public int maxChangeFrequency;
        public int totalTime;
        public bool isComboScoreEnabled;
        public int maxScore;
        public float scorePerCorrectOperation;
        public int comboMultiplier;
    }
}