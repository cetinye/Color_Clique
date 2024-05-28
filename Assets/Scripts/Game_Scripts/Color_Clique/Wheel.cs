using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Color_Clique
{
    public class Wheel : MonoBehaviour
    {
        public int numberOfSlots;
        public Transform center;
        [SerializeField] private Needle needle;
        private WheelPart spawnedWheel;

        [Header("Wheel Prefabs")]
        [SerializeField] private WheelPart wheel4;
        [SerializeField] private WheelPart wheel5;
        [SerializeField] private WheelPart wheel6;
        [SerializeField] private WheelPart wheel8;
        [SerializeField] private WheelPart wheel9;
        [SerializeField] private WheelPart wheel10;
        [SerializeField] private WheelPart wheel12;

        [Header("Lists")]
        [SerializeField] private List<Sprite> items = new List<Sprite>();
        [SerializeField] private List<Sprite> usedItems = new List<Sprite>();

        public void AssignWheelVariables(int numberOfSlots, float needleRotateSpeed)
        {
            this.numberOfSlots = numberOfSlots;
            needle.SetNeedleSpeed(needleRotateSpeed);
        }

        public void Initialize()
        {
            SpawnWheel(numberOfSlots);
        }

        public void SpawnWheel(int numberOfSlots)
        {
            if (spawnedWheel != null)
            {
                Reset();
                Destroy(spawnedWheel.gameObject);
            }

            spawnedWheel = numberOfSlots switch
            {
                4 => Instantiate(wheel4, transform),
                5 => Instantiate(wheel5, transform),
                6 => Instantiate(wheel6, transform),
                8 => Instantiate(wheel8, transform),
                9 => Instantiate(wheel9, transform),
                10 => Instantiate(wheel10, transform),
                12 => Instantiate(wheel12, transform),
                _ => Instantiate(wheel12, transform),
            };

            spawnedWheel.Initialize();
            spawnedWheel.transform.SetSiblingIndex(0);
        }

        public Sprite SelectItem()
        {
            return usedItems[Random.Range(0, usedItems.Count)];
        }

        public void SetNeedleColor(Color color, float duration)
        {
            needle.SetNeedleColor(color, duration);
        }

        public float GetNeedleSpeed()
        {
            return needle.GetNeedleSpeed();
        }

        public Sprite GetSprite()
        {
            return needle.GetOverlappingSlot().GetSprite();
        }

        public Sprite GetRandomItem()
        {
            Sprite item;

            do
            {
                item = items[Random.Range(0, items.Count)];

            } while (usedItems.Contains(item));

            usedItems.Add(item);
            return item;
        }

        private void Reset()
        {
            usedItems.Clear();
        }
    }
}