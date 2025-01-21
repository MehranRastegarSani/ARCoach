using UnityEngine;

public class ArmAlignmentDots : MonoBehaviour
{
    // Left arm (shoulder, elbow, wrist) for both avatars
    public Transform UserAvatar_Left_Shoulder, UserAvatar_Left_Elbow, UserAvatar_Left_Wrist;
    public Transform InstructorAvatar_Left_Shoulder, InstructorAvatar_Left_Elbow, InstructorAvatar_Left_Wrist;

    // Right arm (shoulder, elbow, wrist) for both avatars
    public Transform UserAvatar_Right_Shoulder, UserAvatar_Right_Elbow, UserAvatar_Right_Wrist;
    public Transform InstructorAvatar_Right_Shoulder, InstructorAvatar_Right_Elbow, InstructorAvatar_Right_Wrist;

    // SkinnedMeshRenderers for left and right arms of the UserAvatar
    public SkinnedMeshRenderer UserAvatar_Left_ArmRenderer;
    public SkinnedMeshRenderer UserAvatar_Right_ArmRenderer;

    // Dot texture to overlay on misaligned arms
    public Texture dotTexture;
    public int maxDots = 10; // Maximum number of dots to display when completely misaligned

    // Reference to the custom material
    public Material armAlignmentMaterial;

    void Start()
    {
        // Assign the custom material to the arm renderers
        UserAvatar_Left_ArmRenderer.material = armAlignmentMaterial;
        UserAvatar_Right_ArmRenderer.material = armAlignmentMaterial;

        // Assign the dot texture to the material
        armAlignmentMaterial.SetTexture("_DotTex", dotTexture);
    }

    void Update()
    {
        // Based on the scale, compare arms accordingly
        float leftArmAlignment, rightArmAlignment;

        if (transform.localScale.x > 0)
        {
            // Compare right arm with right arm and left arm with left arm
            leftArmAlignment = CalculateAlignment(
                UserAvatar_Left_Shoulder, UserAvatar_Left_Elbow, UserAvatar_Left_Wrist,
                InstructorAvatar_Left_Shoulder, InstructorAvatar_Left_Elbow, InstructorAvatar_Left_Wrist
            );

            rightArmAlignment = CalculateAlignment(
                UserAvatar_Right_Shoulder, UserAvatar_Right_Elbow, UserAvatar_Right_Wrist,
                InstructorAvatar_Right_Shoulder, InstructorAvatar_Right_Elbow, InstructorAvatar_Right_Wrist
            );
        }
        else
        {
            // Compare right arm with left arm (user's right with instructor's left)
            leftArmAlignment = CalculateAlignment(
                UserAvatar_Right_Shoulder, UserAvatar_Right_Elbow, UserAvatar_Right_Wrist,
                InstructorAvatar_Left_Shoulder, InstructorAvatar_Left_Elbow, InstructorAvatar_Left_Wrist
            );

            rightArmAlignment = CalculateAlignment(
                UserAvatar_Left_Shoulder, UserAvatar_Left_Elbow, UserAvatar_Left_Wrist,
                InstructorAvatar_Right_Shoulder, InstructorAvatar_Right_Elbow, InstructorAvatar_Right_Wrist
            );
        }

		if (transform.localScale.x > 0)
        {
			// Apply the dots overlay based on the alignment
			SetArmMaterialProperties(UserAvatar_Left_ArmRenderer, leftArmAlignment);
			SetArmMaterialProperties(UserAvatar_Right_ArmRenderer, rightArmAlignment);
		}
		else
		{
			// Apply the dots overlay based on the alignment
			SetArmMaterialProperties(UserAvatar_Left_ArmRenderer, rightArmAlignment);
			SetArmMaterialProperties(UserAvatar_Right_ArmRenderer, leftArmAlignment);
		}
    }

    // Method to calculate the alignment between two arms (shoulder → elbow → wrist)
    float CalculateAlignment(Transform userShoulder, Transform userElbow, Transform userWrist, 
                             Transform instructorShoulder, Transform instructorElbow, Transform instructorWrist)
    {
        // Get the vectors for the arm segments of both avatars
        Vector3 userShoulderToElbow = userElbow.position - userShoulder.position;
        Vector3 userElbowToWrist = userWrist.position - userElbow.position;

        Vector3 instructorShoulderToElbow = instructorElbow.position - instructorShoulder.position;
        Vector3 instructorElbowToWrist = instructorWrist.position - instructorElbow.position;

        // Calculate the angles between the corresponding arm segments
        float angleShoulderElbow = Vector3.Angle(userShoulderToElbow, instructorShoulderToElbow);
        float angleElbowWrist = Vector3.Angle(userElbowToWrist, instructorElbowToWrist);

        // Calculate the average alignment based on the angles
        // The smaller the angle, the more aligned the arms are
        float alignment = 1f - (Mathf.Clamp01((angleShoulderElbow + angleElbowWrist) / 180f)); 

        return alignment; // Return value between 0 (misaligned) and 1 (perfectly aligned)
    }

    // Helper method to set the number of dots on the arm's material based on alignment
    void SetArmMaterialProperties(SkinnedMeshRenderer armRenderer, float alignment)
    {
        Material material = armRenderer.material;

        // Ensure the material uses a shader that supports texture blending
        if (material.HasProperty("_DotTiling"))
        {
            // Determine the number of dots to overlay based on alignment
            float dotTiling = Mathf.Lerp(1.0f, maxDots, 1f - alignment);

            // Apply the dot tiling
            material.SetFloat("_DotTiling", dotTiling);
        }
        else
        {
            Debug.LogError("Material on " + armRenderer.name + " doesn't have a DotTiling property!");
        }
    }
}
