using UnityEngine;

public class JointAlignmentBrightness : MonoBehaviour
{
    // Left arm (elbow, wrist) for both avatars
    public Transform UserAvatar_Left_Shoulder, UserAvatar_Left_Elbow, UserAvatar_Left_Wrist;
    public Transform InstructorAvatar_Left_Shoulder, InstructorAvatar_Left_Elbow, InstructorAvatar_Left_Wrist;

    // Right arm (elbow, wrist) for both avatars
    public Transform UserAvatar_Right_Shoulder, UserAvatar_Right_Elbow, UserAvatar_Right_Wrist;
    public Transform InstructorAvatar_Right_Shoulder, InstructorAvatar_Right_Elbow, InstructorAvatar_Right_Wrist;

    // Joint markers for left and right arm (these could be spheres or any other visual indicators)
    public GameObject Left_Elbow_Sphere, Left_Wrist_Sphere;
    public GameObject Right_Elbow_Sphere, Right_Wrist_Sphere;

    // Offset value to move the markers in front of the body
    public float markerOffset = 0.2f; // Adjust this value to get the correct position
    
    // Brightness settings (Wider range for more distinction)
    public float maxBrightness = 3f;  // Maximum brightness when arms are misaligned
    public float minBrightness = 0.1f; // Minimum brightness when arms are fully aligned

    // Separate max distances for elbow and wrist
    public float maxElbowDistance = 2f; // Max distance for elbow
    public float maxWristDistance = 2f; // Max distance for wrist
    
    private Renderer Left_Elbow_Marker;
    private Renderer Right_Elbow_Marker;
    private Renderer Left_Wrist_Marker;
    private Renderer Right_Wrist_Marker;

    void Start() 
    { 
        Left_Elbow_Sphere.SetActive(true);
        Right_Elbow_Sphere.SetActive(true);
        Left_Wrist_Sphere.SetActive(true);
        Right_Wrist_Sphere.SetActive(true);
        
        Left_Elbow_Marker = Left_Elbow_Sphere.GetComponent<MeshRenderer>();
        Right_Elbow_Marker = Right_Elbow_Sphere.GetComponent<MeshRenderer>();
        Left_Wrist_Marker = Left_Wrist_Sphere.GetComponent<MeshRenderer>();
        Right_Wrist_Marker = Right_Wrist_Sphere.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (transform.localScale.x > 0)
        {
            // Left Arm Alignment (only elbow and wrist)
            UpdateJointBrightness(UserAvatar_Left_Elbow, InstructorAvatar_Left_Elbow, Left_Elbow_Marker, maxElbowDistance);
            UpdateJointBrightness(UserAvatar_Left_Wrist, InstructorAvatar_Left_Wrist, Left_Wrist_Marker, maxWristDistance);

            // Right Arm Alignment (only elbow and wrist)
            UpdateJointBrightness(UserAvatar_Right_Elbow, InstructorAvatar_Right_Elbow, Right_Elbow_Marker, maxElbowDistance);
            UpdateJointBrightness(UserAvatar_Right_Wrist, InstructorAvatar_Right_Wrist, Right_Wrist_Marker, maxWristDistance);
        }
        else
        {
            // Left Arm Alignment (only elbow and wrist)
            UpdateJointBrightness(UserAvatar_Left_Elbow, InstructorAvatar_Right_Elbow, Left_Elbow_Marker, maxElbowDistance);
            UpdateJointBrightness(UserAvatar_Left_Wrist, InstructorAvatar_Right_Wrist, Left_Wrist_Marker, maxWristDistance);

            // Right Arm Alignment (only elbow and wrist)
            UpdateJointBrightness(UserAvatar_Right_Elbow, InstructorAvatar_Left_Elbow, Right_Elbow_Marker, maxElbowDistance);
            UpdateJointBrightness(UserAvatar_Right_Wrist, InstructorAvatar_Left_Wrist, Right_Wrist_Marker, maxWristDistance);
        }

    }

    // Update the brightness of the joint marker based on alignment
    void UpdateJointBrightness(Transform userJoint, Transform instructorJoint, Renderer jointMarker, float maxJointDistance)
    {
        // Calculate alignment between user joint and instructor joint
        float alignment = CalculateAlignment(userJoint, instructorJoint, maxJointDistance);

        // Calculate brightness based on alignment (low brightness for alignment, high for misalignment)
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, 1f - alignment);

        // Adjust the material color's brightness using HSV
        if (jointMarker != null)
        {
            // Get the current color of the material
            Color currentColor = jointMarker.material.color;

            // Convert the color to HSV
            Color.RGBToHSV(currentColor, out float h, out float s, out float v);

            // Use the calculated brightness to set the value (V) for brightness
            v = brightness; // Apply the brightness value directly here

            // Convert it back to RGB
            Color newColor = Color.HSVToRGB(h, s, v);

            // Apply the new color to the material
            jointMarker.material.color = newColor;
        }
    }

    // Calculate alignment between the user's joint and the instructor's joint
    float CalculateAlignment(Transform userJoint, Transform instructorJoint, float maxJointDistance)
    {
        // Get the vectors from the shoulder to the elbow, and from the elbow to the wrist for both user and instructor
        Vector3 userShoulderToElbow = userJoint.position - userJoint.parent.position;
        Vector3 userElbowToWrist = userJoint.position - userJoint.parent.position;

        Vector3 instructorShoulderToElbow = instructorJoint.position - instructorJoint.parent.position;
        Vector3 instructorElbowToWrist = instructorJoint.position - instructorJoint.parent.position;

        // Calculate the angles between the user and instructor's joint vectors
        float angleShoulderElbow = Vector3.Angle(userShoulderToElbow, instructorShoulderToElbow);
        float angleElbowWrist = Vector3.Angle(userElbowToWrist, instructorElbowToWrist);

        // Calculate the average angle
        float averageAngle = (angleShoulderElbow + angleElbowWrist) / 2f;

        // Normalize the angle (the smaller the angle, the better the alignment)
        float normalizedAngle = Mathf.Clamp01(1f - (averageAngle / 180f));

        // Now calculate the distance between the user joint and the instructor joint
        float distance = Vector3.Distance(userJoint.position, instructorJoint.position);

        // Normalize the distance (the smaller the distance, the better the alignment)
        float normalizedDistance = Mathf.Clamp01(1f - Mathf.InverseLerp(0f, maxJointDistance, distance));

        // Combine both distance and angle into a final alignment score
        // We take a weighted average of both the distance and angle scores to get a final alignment value
        float combinedAlignment = (normalizedAngle + normalizedDistance) / 2f;

        return combinedAlignment; // Return value between 0 (misaligned) and 1 (perfectly aligned)
    }


}
