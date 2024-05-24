using UnityEngine;

namespace Color_Clique
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObjects/LevelSO", order = 1)]
    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public int slotCount;
        public int rotationSpeed;
        public int needleRotateSpeed;
        public int newItemInterval;
        public int time;
    }
}