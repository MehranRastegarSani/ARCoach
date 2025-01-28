using System.Collections;
using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    public Transform rightBalloon;
    public Transform leftBalloon;
	
	public GameObject RightHandTrigger;  // Reference to the sphere object
	public GameObject LeftHandTrigger;  // Reference to the sphere object
	
	public GameObject InstructorAvatar;
	private Animator animator;	
	
    public Vector3[] rightBalloonPositions;
    public Vector3[] leftBalloonPositions;

	
	public Vector3[] rightBalloonPositionsSwitchSide;
    public Vector3[] leftBalloonPositionsSwitchSide;
    public float fadeDuration = 0.2f;  // Duration for fade in/out


    private Renderer rightBalloonRenderer;
    private Renderer leftBalloonRenderer;
	
	private Collider rightBalloonCollider;  // Collider for RightBalloon
    private Collider RightHandCollider;  // Collider for the sphere
	
	private Collider leftBalloonCollider;  // Collider for RightBalloon
    private Collider LeftHandCollider;  // Collider for the sphere
	
	private bool isBalloonMoving = false;
	
	private int positionIndex = 0;
	
	private float currentAvatarSide = 0;  // Duration for fade in/out
	
	private int currentForwarded = 0;
	
	public ControlOptions ControlOptionsReference;
	
	


    void Start()
    {
		
		rightBalloon.gameObject.SetActive(true);
		leftBalloon.gameObject.SetActive(true);
		
		currentAvatarSide = transform.localScale.x;
		
		
        // Get the Renderer components (e.g., MeshRenderer) from your balloons
        rightBalloonRenderer = rightBalloon.GetComponent<Renderer>();
        leftBalloonRenderer = leftBalloon.GetComponent<Renderer>();
		
		rightBalloonCollider = rightBalloon.GetComponent<Collider>();
        RightHandCollider = RightHandTrigger.GetComponent<Collider>();
		
		leftBalloonCollider = leftBalloon.GetComponent<Collider>();
        LeftHandCollider = LeftHandTrigger.GetComponent<Collider>();
		
		currentForwarded = ControlOptionsReference.Forwarded;

    }
	
	void Update()
    {
        // Check if RightBalloon's collider is inside the sphere's collider
        if ((IsBalloonOnTrigger() || currentAvatarSide != transform.localScale.x) && !isBalloonMoving)
        {
			StartCoroutine(MoveBalloonOnTrigger());
			currentAvatarSide = transform.localScale.x;
        }
		
		animator = InstructorAvatar.GetComponent<Animator>();

		
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.001)
		{
			
			if (transform.localScale.x > 0)
			{
				rightBalloon.position = leftBalloonPositionsSwitchSide[leftBalloonPositionsSwitchSide.Length - 1];
				leftBalloon.position = rightBalloonPositionsSwitchSide[rightBalloonPositionsSwitchSide.Length - 1];
				positionIndex = 0;
			}
			else
			{
				rightBalloon.position = leftBalloonPositions[leftBalloonPositions.Length - 1];
				leftBalloon.position = rightBalloonPositions[rightBalloonPositions.Length - 1];						
				positionIndex = 0;
			}	
		}
		
		if ( currentForwarded < ControlOptionsReference.Forwarded && !isBalloonMoving)
        {
			StartCoroutine(MoveBalloonOnTrigger());
			currentAvatarSide = transform.localScale.x;
			currentForwarded = ControlOptionsReference.Forwarded;
        }
		else if ( currentForwarded > ControlOptionsReference.Forwarded && !isBalloonMoving)
		{

			if (positionIndex > 1)
			{
				positionIndex = (positionIndex - 2) % rightBalloonPositions.Length;
				StartCoroutine(MoveBalloonOnTrigger());
				currentAvatarSide = transform.localScale.x;
			}
			currentForwarded = ControlOptionsReference.Forwarded;
		}
		
		
    }

    // Check if the RightBalloon's collider intersects with the sphere's collider
    bool IsBalloonOnTrigger()
    {
		bool rightHandIntersects = RightHandCollider != null && RightHandCollider.bounds.Intersects(rightBalloonCollider.bounds);
        bool leftHandIntersects = LeftHandCollider != null && LeftHandCollider.bounds.Intersects(leftBalloonCollider.bounds);
		
		return rightHandIntersects && leftHandIntersects;
        return false;
    }
	
	
	
	IEnumerator MoveBalloonOnTrigger()
    {
		isBalloonMoving = true;
		if (transform.localScale.x > 0)
		{
			// Fade out, move, and fade in both balloons simultaneously
			yield return StartCoroutine(FadeAndMoveBothBalloons(rightBalloon, leftBalloon, leftBalloonPositionsSwitchSide[positionIndex], rightBalloonPositionsSwitchSide[positionIndex], fadeDuration));
		}
		else
		{
			// Fade out, move, and fade in both balloons simultaneously
			yield return StartCoroutine(FadeAndMoveBothBalloons(rightBalloon, leftBalloon, leftBalloonPositions[positionIndex], rightBalloonPositions[positionIndex], fadeDuration));
		}
		
		// Move to the next position in the array, looping back if at the end
		positionIndex = (positionIndex + 1) % rightBalloonPositions.Length;
		isBalloonMoving = false;

    }
	


    // This method will move both the right and left balloons at the same time
    IEnumerator FadeAndMoveBothBalloons(Transform rightBalloon, Transform leftBalloon, Vector3 rightTargetPosition, Vector3 leftTargetPosition, float fadeDuration)
    {
        // Fade out both balloons and disable their renderers (make them invisible)
        yield return StartCoroutine(FadeBalloon(rightBalloonRenderer, 1f, 0f));  // Fade to invisible for right balloon
        yield return StartCoroutine(FadeBalloon(leftBalloonRenderer, 1f, 0f));   // Fade to invisible for left balloon

        rightBalloonRenderer.enabled = false;  // Disable the renderer of right balloon
        leftBalloonRenderer.enabled = false;   // Disable the renderer of left balloon

        // Move both balloons to their new positions while they are invisible
        Vector3 rightInitialPosition = rightBalloon.position;
        Vector3 leftInitialPosition = leftBalloon.position;
        float timeElapsed = 0f;

        // Move both balloons simultaneously
        while (timeElapsed < fadeDuration)
        {
            rightBalloon.position = Vector3.Lerp(rightInitialPosition, rightTargetPosition, timeElapsed / fadeDuration);
            leftBalloon.position = Vector3.Lerp(leftInitialPosition, leftTargetPosition, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure both balloons reach their target positions
        rightBalloon.position = rightTargetPosition;
        leftBalloon.position = leftTargetPosition;

        // Re-enable the renderers (making both balloons visible again)
        rightBalloonRenderer.enabled = true;
        leftBalloonRenderer.enabled = true;

        // Fade both balloons in simultaneously
        yield return StartCoroutine(FadeBalloon(rightBalloonRenderer, 0f, 1f));  // Fade to visible for right balloon
        yield return StartCoroutine(FadeBalloon(leftBalloonRenderer, 0f, 1f));   // Fade to visible for left balloon
    }

    IEnumerator FadeBalloon(Renderer renderer, float startAlpha, float targetAlpha)
    {
        float timeElapsed = 0f;
        Color initialColor = renderer.material.color;
        float currentAlpha = startAlpha;

        // Smoothly change the alpha value over time for fade in/out
        while (timeElapsed < fadeDuration)
        {
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, currentAlpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha value is set correctly
        renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
    }
}