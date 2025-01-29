using UnityEngine;

public class ArmAlignmentColor : MonoBehaviour
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

    // Transparency control value (0 = fully transparent, 1 = fully opaque)
    [Range(0, 1)] public float maxTransparency = 0.5f; 
	
	private SkinnedMeshRenderer originalLeftArmRenderer;
	private SkinnedMeshRenderer originalRightArmRenderer;
	

    void Start()
    {
		originalLeftArmRenderer = UserAvatar_Left_ArmRenderer;
		originalRightArmRenderer = UserAvatar_Right_ArmRenderer;

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

        // Map the alignment values (0 to 1) to a color range (red to green)
        Color leftArmColor = Color.Lerp(Color.red, Color.green, leftArmAlignment);
        Color rightArmColor = Color.Lerp(Color.red, Color.green, rightArmAlignment);

        // Calculate transparency based on alignment (the more aligned, the more transparent)
        float leftArmTransparency = Mathf.Lerp(0, maxTransparency, leftArmAlignment);
        float rightArmTransparency = Mathf.Lerp(0, maxTransparency, rightArmAlignment);

        if (transform.localScale.x > 0)
        {
            // Apply the heatmap overlay and preserve the original skin for the respective arm SkinnedMeshRenderers
            SetArmMaterialProperties(UserAvatar_Left_ArmRenderer, leftArmColor, leftArmTransparency);
            SetArmMaterialProperties(UserAvatar_Right_ArmRenderer, rightArmColor, rightArmTransparency);
        }
        else
        {
            // Apply the heatmap overlay and preserve the original skin for the respective arm SkinnedMeshRenderers
            SetArmMaterialProperties(UserAvatar_Right_ArmRenderer, leftArmColor, leftArmTransparency);
            SetArmMaterialProperties(UserAvatar_Left_ArmRenderer, rightArmColor, rightArmTransparency);
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

    // Helper method to set the color and transparency of the arm's material while preserving the skin texture
    void SetArmMaterialProperties(SkinnedMeshRenderer armRenderer, Color armColor, float transparency)
    {
        Material material = armRenderer.material;

        // Make sure the material uses a shader that supports transparency and texture blending
        if (material.HasProperty("_MainTex"))
        {
            // Enable transparency and blend the color with the original texture
            material.SetFloat("_Mode", 3);  // Set to Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;  // Transparent render queue

            // Blend the heatmap color with the arm texture
            Color colorWithTransparency = armColor;
            colorWithTransparency.a = 1f - transparency;  // More aligned = more transparent

            material.SetColor("_Color", colorWithTransparency); // Set color for the heatmap overlay

            // The original texture remains unchanged
            material.SetTexture("_MainTex", armRenderer.sharedMaterial.GetTexture("_MainTex"));
        }
        else
        {
            Debug.LogError("Material on " + armRenderer.name + " doesn't have a MainTex property!");
        }
    }
	
	
	// public void Destroying()
	// {
		// Debug.LogWarning("Hi");
		// UserAvatar_Left_ArmRenderer = originalLeftArmRenderer;
		// UserAvatar_Right_ArmRenderer = originalRightArmRenderer;
	// }
	
	
	
	public void Destroying()
{
    Debug.LogWarning("Destroying the color effect...");

    // Reset the material properties to the original state
    ResetArmMaterialProperties(UserAvatar_Left_ArmRenderer);
    ResetArmMaterialProperties(UserAvatar_Right_ArmRenderer);

    // Reset the SkinnedMeshRenderer references (if needed)
    UserAvatar_Left_ArmRenderer = originalLeftArmRenderer;
    UserAvatar_Right_ArmRenderer = originalRightArmRenderer;

    // Any other necessary reset logic goes here
}

// Reset the material properties to their original state
	void ResetArmMaterialProperties(SkinnedMeshRenderer armRenderer)
	{
		Material material = armRenderer.material;

		if (material.HasProperty("_MainTex"))
		{
			// Reset to default opaque mode
			material.SetFloat("_Mode", 0);  // Set to Opaque mode (0 = Opaque, 3 = Transparent)
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt("_ZWrite", 1);  // Enable writing to depth buffer
			material.EnableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = -1;  // Default render queue for opaque materials

			// Restore the original color (if you saved it earlier)
			material.SetColor("_Color", Color.white);  // Or the original color if you had one saved
		}
		else
		{
			Debug.LogError("Material on " + armRenderer.name + " doesn't have a MainTex property!");
		}
	}
}