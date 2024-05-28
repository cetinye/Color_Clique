using System.Collections.Generic;
using UnityEngine;

namespace Color_Clique
{
    public class WheelPart : MonoBehaviour
    {
        [SerializeField] private List<Slot> slots = new List<Slot>();

        public void Initialize()
        {
            foreach (Slot slot in slots)
            {
                slot.SetSprite(LevelManager.instance.GetWheel().GetRandomItem());
            }
        }
    }
}