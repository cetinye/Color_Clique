using UnityEngine;

public class LuckyWheel : MonoBehaviour
{
    public GameObject slotPrefab; // Prefab for the wheel slots
    public int numberOfSlots; // Number of slots on the wheel
    public float wheelRadius = 5f; // Radius of the wheel
    public Transform spawnTransform;

    void Start()
    {
        CreateWheel();
    }

    void CreateWheel()
    {
        float angleStep = 360f / numberOfSlots;

        for (int i = 0; i < numberOfSlots; i++)
        {
            float angle = i * angleStep;
            float angleInRadians = angle * Mathf.Deg2Rad;
            float xPosition = Mathf.Cos(angleInRadians) * wheelRadius;
            float yPosition = Mathf.Sin(angleInRadians) * wheelRadius;

            Vector3 slotPosition = new Vector3(xPosition, yPosition, 0f);
            GameObject slot = Instantiate(slotPrefab, slotPosition, Quaternion.identity, spawnTransform);

            // Rotate the slot to face outward
            slot.transform.eulerAngles = new Vector3(0f, 0f, angle - 90f);
        }
    }
}
