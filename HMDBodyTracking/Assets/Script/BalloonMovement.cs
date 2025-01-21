using System.Collections;
using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    public Transform rightBalloon;
    public Transform leftBalloon;
    public Vector3[] rightBalloonPositions;
    public Vector3[] leftBalloonPositions;
    public float[] positionDurations;  // Duration for each position in seconds
    public float fadeDuration = 1f;    // Duration for fade in/out

	private Renderer rightBalloonRenderer;
    private Renderer leftBalloonRenderer;

    void Start()
    {
        // Get the Renderer components (e.g., MeshRenderer) from your balloons
        rightBalloonRenderer = rightBalloon.GetComponent<Renderer>();
        leftBalloonRenderer = leftBalloon.GetComponent<Renderer>();

        // Start the animation coroutine
        StartCoroutine(MoveAndFadeBalloons());
    }

    IEnumerator MoveAndFadeBalloons()
    {
        for (int i = 0; i < rightBalloonPositions.Length; i++)
        {
            // Fade out and move the right balloon to its new position
            yield return StartCoroutine(FadeAndMoveBalloon(rightBalloon, rightBalloonPositions[i], positionDurations[i], rightBalloonRenderer));

            // Fade out and move the left balloon to its new position
            yield return StartCoroutine(FadeAndMoveBalloon(leftBalloon, leftBalloonPositions[i], positionDurations[i], leftBalloonRenderer));

            // Wait for the duration time for this set of positions
            yield return new WaitForSeconds(positionDurations[i]);
        }
    }

    IEnumerator FadeAndMoveBalloon(Transform balloon, Vector3 targetPosition, float duration, Renderer renderer)
    {
        // Fade out the balloon before moving it
        yield return FadeBalloon(renderer, 1f, 0f);

        // Move the balloon smoothly towards the target position
        Vector3 initialPosition = balloon.position;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            balloon.position = Vector3.Lerp(initialPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the balloon reaches the target position
        balloon.position = targetPosition;

        // Fade in the balloon after reaching the new position
        yield return FadeBalloon(renderer, 0f, 1f);
    }

    IEnumerator FadeBalloon(Renderer renderer, float startAlpha, float targetAlpha)
    {
        float timeElapsed = 0f;
        Color initialColor = renderer.material.color;
        float currentAlpha = startAlpha;

        while (timeElapsed < fadeDuration)
        {
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, currentAlpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha reaches the target value
        renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
    }
}