using UnityEngine;

public class ArmAlignmentOutline : MonoBehaviour
{
    // Left arm (shoulder, elbow, wrist) for both avatars
    public Transform UserAvatar_Left_Shoulder, UserAvatar_Left_Elbow, UserAvatar_Left_Wrist;
    public Transform InstructorAvatar_Left_Shoulder, InstructorAvatar_Left_Elbow, InstructorAvatar_Left_Wrist;

    // Right arm (shoulder, elbow, wrist) for both avatars
    public Transform UserAvatar_Right_Shoulder, UserAvatar_Right_Elbow, UserAvatar_Right_Wrist;
    public Transform InstructorAvatar_Right_Shoulder, InstructorAvatar_Right_Elbow, InstructorAvatar_Right_Wrist;
	
	
    // QuickOutline references
    public Outline LeftArmOutline;
    public Outline RightArmOutline;

    // Transparency control value (0 = fully transparent, 1 = fully opaque)
    [Range(0, 1)] public float outlineTransparency = 0.5f;  // Transparency control value (0 = fully transparent, 1 = fully opaque)


	void Update()
    {
		
		if (!LeftArmOutline.enabled)
		{
			LeftArmOutline.enabled = true;
			RightArmOutline.enabled = true;
		}
		
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

        // Map alignment values (0 to 1) to colors (red to green)
        Color leftArmColor = Color.Lerp(Color.red, Color.green, leftArmAlignment);
        Color rightArmColor = Color.Lerp(Color.red, Color.green, rightArmAlignment);


        // Adjust the alpha (transparency) of the outline color based on the slider value
        leftArmColor.a = outlineTransparency;
        rightArmColor.a = outlineTransparency;


		
		if (transform.localScale.x > 0)
        {
			// Apply the outline color based on arm alignment and transparency
			LeftArmOutline.OutlineColor = leftArmColor;
			RightArmOutline.OutlineColor = rightArmColor;
        }
        else
        {
			// Apply the outline color based on arm alignment and transparency
			LeftArmOutline.OutlineColor = rightArmColor;
			RightArmOutline.OutlineColor = leftArmColor;
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
	
	public void Destroying()
	{
		LeftArmOutline.enabled = false;
        RightArmOutline.enabled = false;
	}
}