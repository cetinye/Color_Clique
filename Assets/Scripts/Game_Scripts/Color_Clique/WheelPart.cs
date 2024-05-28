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
                slot.SetItemSprite(LevelManager.instance.GetWheel().GetRandomItem());
                slot.SetSlotColor(LevelManager.instance.GetWheel().GetRandomColor());

                LevelManager.instance.GetWheel().ResetColors();
            }
        }

        public Slot GetRandomSlot()
        {
            return slots[Random.Range(0, slots.Count)];
        }
    }
}