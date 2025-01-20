using UnityEngine;

public class JointAlignmentSize : MonoBehaviour
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

    // Sensitivity for distance-based scaling
    public float maxScale = 2f; // Maximum scale for the joint marker (when perfectly aligned)
    public float minScale = 0.5f; // Minimum scale for the joint marker (when misaligned)

    // Max distances for alignment (separate for elbow and wrist)
    public float maxElbowDistance = 2f; // Max possible distance for elbow misalignment
    public float maxWristDistance = 2f; // Max possible distance for wrist misalignment


    void Start() 
    { 
        Left_Elbow_Sphere.SetActive(true);
        Right_Elbow_Sphere.SetActive(true);
        Left_Wrist_Sphere.SetActive(true);
        Right_Wrist_Sphere.SetActive(true);

    }

    void Update()
    {
        if (transform.localScale.x > 0)
        {
            // Left Arm Alignment (only elbow and wrist)
            UpdateJointScale(UserAvatar_Left_Elbow, InstructorAvatar_Left_Elbow, Left_Elbow_Sphere, true);
            UpdateJointScale(UserAvatar_Left_Wrist, InstructorAvatar_Left_Wrist, Left_Wrist_Sphere, false);

            // Right Arm Alignment (only elbow and wrist)
            UpdateJointScale(UserAvatar_Right_Elbow, InstructorAvatar_Right_Elbow, Right_Elbow_Sphere, true);
            UpdateJointScale(UserAvatar_Right_Wrist, InstructorAvatar_Right_Wrist, Right_Wrist_Sphere, false);
        }
        else
        {
            // Left Arm Alignment (only elbow and wrist)
            UpdateJointScale(UserAvatar_Left_Elbow, InstructorAvatar_Right_Elbow, Left_Elbow_Sphere, true);
            UpdateJointScale(UserAvatar_Left_Wrist, InstructorAvatar_Right_Wrist, Left_Wrist_Sphere, false);

            // Right Arm Alignment (only elbow and wrist)
            UpdateJointScale(UserAvatar_Right_Elbow, InstructorAvatar_Left_Elbow, Right_Elbow_Sphere, true);
            UpdateJointScale(UserAvatar_Right_Wrist, InstructorAvatar_Left_Wrist, Right_Wrist_Sphere, false);
        }

    }

    // Update the scale of the joint marker based on alignment
    void UpdateJointScale(Transform userJoint, Transform instructorJoint, GameObject jointMarker, bool isElbow)
    {
        // Calculate alignment between user joint and instructor joint
        float alignment = CalculateAlignment(userJoint, instructorJoint, isElbow);

        // Calculate the scale based on alignment (from maxScale to minScale)
        float jointScale = Mathf.Lerp(maxScale, minScale, alignment);

        // Apply the scale to the joint marker (which should be a GameObject with a Transform)
        if (jointMarker != null)
        {
            jointMarker.transform.localScale = new Vector3(jointScale, jointScale, jointScale);
        }
    }

    // Calculate alignment between the user's joint and the instructor's joint
    float CalculateAlignment(Transform userJoint, Transform instructorJoint, bool isElbow)
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

        // Use the appropriate max distance based on whether it's the elbow or wrist
        float maxJointDistance = isElbow ? maxElbowDistance : maxWristDistance;

        // Normalize the distance (the smaller the distance, the better the alignment)
        float normalizedDistance = Mathf.Clamp01(1f - Mathf.InverseLerp(0f, maxJointDistance, distance)); // Use the appropriate max distance

        // Combine both distance and angle into a final alignment score
        // We take a weighted average of both the distance and angle scores to get a final alignment value
        float combinedAlignment = (normalizedAngle + normalizedDistance) / 2f;

        return combinedAlignment; // Return value between 0 (misaligned) and 1 (perfectly aligned)
    }
}
