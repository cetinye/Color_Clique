using UnityEngine;

public class Wheel : MonoBehaviour
{
    public GameObject slotPrefab; // Prefab for the wheel slots
    public int numberOfSlots; // Number of slots on the wheel
    public float wheelRadius = 5f; // Radius of the wheel
    public Transform spawnTransform; // The transform that will hold the slots

    void Start()
    {
        CreateWheel(); // Calls the method to create the wheel when the script starts
    }

    void CreateWheel()
    {
        float angleStep = 360f / numberOfSlots; // The angle between each slot
        float slotWidth = 2f * wheelRadius * Mathf.Tan(Mathf.PI / numberOfSlots); // Calculates the width of each slot so they fit without gaps
        float slotHeight = wheelRadius; // Assuming slot height equals radius for proper fit

        for (int i = 0; i < numberOfSlots; i++)
        {
            float angle = i * angleStep; // Current angle for the slot
            float angleInRadians = angle * Mathf.Deg2Rad; // Convert the angle to radians for trigonometric functions
            float xPosition = Mathf.Cos(angleInRadians) * wheelRadius; // X position of the slot
            float yPosition = Mathf.Sin(angleInRadians) * wheelRadius; // Y position of the slot

            Vector3 slotPosition = new Vector3(xPosition, yPosition, 0f); // Position vector for the slot
            GameObject slot = Instantiate(slotPrefab, slotPosition, Quaternion.identity, spawnTransform); // Instantiate the slot at the calculated position

            // Adjust the scale of the slot to fit correctly
            slot.transform.localScale = new Vector3((slotWidth / slot.GetComponent<Renderer>().bounds.size.x) * 3f, (slotHeight / slot.GetComponent<Renderer>().bounds.size.y) * 2.8f, 1f);

            // Rotate the slot to face outward
            slot.transform.eulerAngles = new Vector3(0f, 0f, angle - 90f);
        }

        // Move the slots' parent transform upwards
        spawnTransform.localPosition = new Vector3(0f, 3.78f, 0f);
    }
}
