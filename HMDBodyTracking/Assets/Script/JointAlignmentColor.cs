using UnityEngine;

public class JointAlignmentColor : MonoBehaviour
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
	
	// Sensitivity for distance-based color change
    public float distanceThreshold = 0.5f; // Distance at which the joint turns fully red
    public float maxDistance = 2f; // Max possible distance for 100% misalignment
	
	
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
			UpdateJointColor(UserAvatar_Left_Elbow, InstructorAvatar_Left_Elbow, Left_Elbow_Marker);
			UpdateJointColor(UserAvatar_Left_Wrist, InstructorAvatar_Left_Wrist, Left_Wrist_Marker);

			// Right Arm Alignment (only elbow and wrist)
			UpdateJointColor(UserAvatar_Right_Elbow, InstructorAvatar_Right_Elbow, Right_Elbow_Marker);
			UpdateJointColor(UserAvatar_Right_Wrist, InstructorAvatar_Right_Wrist, Right_Wrist_Marker);
		}
		else
		{
			// Left Arm Alignment (only elbow and wrist)
			UpdateJointColor(UserAvatar_Left_Elbow, InstructorAvatar_Right_Elbow, Left_Elbow_Marker);
			UpdateJointColor(UserAvatar_Left_Wrist, InstructorAvatar_Right_Wrist, Left_Wrist_Marker);

			// Right Arm Alignment (only elbow and wrist)
			UpdateJointColor(UserAvatar_Right_Elbow, InstructorAvatar_Left_Elbow, Right_Elbow_Marker);
			UpdateJointColor(UserAvatar_Right_Wrist, InstructorAvatar_Left_Wrist, Right_Wrist_Marker);
		}
        // Position the markers in front of the body
        PositionMarkersInFront(UserAvatar_Left_Elbow, Left_Elbow_Marker);
        PositionMarkersInFront(UserAvatar_Left_Wrist, Left_Wrist_Marker);
        PositionMarkersInFront(UserAvatar_Right_Elbow, Right_Elbow_Marker);
        PositionMarkersInFront(UserAvatar_Right_Wrist, Right_Wrist_Marker);

        // Adjust render queue to ensure markers are always on top of the arms
        SetMarkerRenderQueue(Left_Elbow_Marker, 3000);  // Higher queue than default (2000)
        SetMarkerRenderQueue(Left_Wrist_Marker, 3000);
        SetMarkerRenderQueue(Right_Elbow_Marker, 3000);
        SetMarkerRenderQueue(Right_Wrist_Marker, 3000);
    }

    // Update the color of the joint marker based on alignment
    void UpdateJointColor(Transform userJoint, Transform instructorJoint, Renderer jointMarker)
    {
        // Calculate alignment between user joint and instructor joint
        float alignment = CalculateAlignment(userJoint, instructorJoint);

        // Get the color based on alignment (from red to green)
        Color jointColor = Color.Lerp(Color.red, Color.green, alignment);

        // Apply the color to the joint marker (which should be a Renderer attached to a visual marker)
        if (jointMarker != null)
        {
            jointMarker.material.color = jointColor;
        }
    }

    // Calculate alignment between the user's joint and the instructor's joint
	float CalculateAlignment(Transform userJoint, Transform instructorJoint)
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
        float normalizedDistance = Mathf.Clamp01(1f - Mathf.InverseLerp(0f, maxDistance, distance));

        // Combine both distance and angle into a final alignment score
        // We take a weighted average of both the distance and angle scores to get a final alignment value
        float combinedAlignment = (normalizedAngle + normalizedDistance) / 2f;

        return combinedAlignment; // Return value between 0 (misaligned) and 1 (perfectly aligned)
    }


    // Move the marker in front of the body by applying an offset in the forward direction
    void PositionMarkersInFront(Transform joint, Renderer jointMarker)
    {
        if (jointMarker != null)
        {
            // Apply an offset in the direction the joint is facing (using the forward vector)
            Vector3 offsetPosition = joint.position + joint.forward * markerOffset;

            // Move the marker to the new position
            jointMarker.transform.position = offsetPosition;
        }
    }

    // Set the render queue of the marker material to a higher value so it renders on top of other objects
    void SetMarkerRenderQueue(Renderer markerRenderer, int renderQueue)
    {
        if (markerRenderer != null && markerRenderer.material != null)
        {
            // Set the render queue for the material to ensure it renders last (after arms)
            markerRenderer.material.renderQueue = renderQueue;
        }
    }
}
