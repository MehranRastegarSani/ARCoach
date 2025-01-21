using UnityEngine;

public class WristDebug : MonoBehaviour
{
    public Transform UserAvatar_Right_Wrist;
    public Transform UserAvatar_Left_Wrist;

    void Update()
    {
        // Check if any key is pressed (e.g., Space key)
        if (Input.anyKeyDown)
        {
            DebugWristPositions();
        }
    }

    void DebugWristPositions()
    {
        if (UserAvatar_Right_Wrist != null)
        {
            Debug.Log("Right Wrist Position: " + UserAvatar_Right_Wrist.position);
        }
        else
        {
            Debug.LogWarning("UserAvatar_Right_Wrist is not assigned!");
        }

        if (UserAvatar_Left_Wrist != null)
        {
            Debug.Log("Left Wrist Position: " + UserAvatar_Left_Wrist.position);
        }
        else
        {
            Debug.LogWarning("UserAvatar_Left_Wrist is not assigned!");
        }
    }
}
