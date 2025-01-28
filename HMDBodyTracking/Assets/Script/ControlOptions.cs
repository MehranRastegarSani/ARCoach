using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ControlOptions : MonoBehaviour
{
    public GameObject InstructorAvatar;  // Reference to the InstructorInstructorAvatar GameObject
    private Animator animator;
	public GameObject UserAvatar; 
	
	public GameObject userAvatarOld;
	
	public int Forwarded = 0;
	
    private bool isPaused = false;
	GameObject presser;
	AudioSource sound;
	private float animationTime = 0f;
	private float CurrentAnimationSpeed = 0f;
	// private float SpeedChanged = 0f;
	public TextMeshProUGUI SpeedUpText;
	public TextMeshProUGUI SlowDownText;
	public TextMeshProUGUI TimeFrame;
	public GameObject PauseText; 
	public GameObject ResumeText; 
	
	
	public MonoBehaviour script3; // Toggleable
    public MonoBehaviour script4; // Toggleable
    public MonoBehaviour script5; // Toggleable
    public MonoBehaviour script6; // Toggleable
    public MonoBehaviour script7; // Toggleable
	
	private float clickCooldown = 1f;  // Time to prevent multiple clicks (0.2 seconds)
    private float lastClickTime = 0f;
	
	// private int currentMode = 0;
	

	
    void Start()
    {
		sound = GetComponent<AudioSource>();
        // Ensure that the InstructorInstructorAvatar GameObject is assigned in the Inspector
        if (InstructorAvatar != null)
        {
            // Get the Animator component from the InstructorInstructorAvatar GameObject
            animator = InstructorAvatar.GetComponent<Animator>();
			CurrentAnimationSpeed = animator.speed;
        }
        else
        {
            Debug.LogError("InstructorInstructorAvatar GameObject is not assigned in the Inspector!");
        }
    }
	
	
	void Update()
    {
		if (InstructorAvatar != null)
        {
			// Show animation speed
			if (CurrentAnimationSpeed > 1)
			{
				SlowDownText.text = null;
			    float newSpeed = (float)Math.Round((CurrentAnimationSpeed - 1), 1);
				SpeedUpText.text = "+" + newSpeed.ToString();

			}
			else if (CurrentAnimationSpeed < 1)
			{
				SpeedUpText.text = null;
			    float newSpeed = (float)Math.Round((CurrentAnimationSpeed - 1), 1);
				SlowDownText.text = newSpeed.ToString();		
			}
			else
			{
				SlowDownText.text = null;
				SpeedUpText.text = null;
			}
			
			// Show the Time Frame
			if (InstructorAvatar.activeSelf)
			{			
				AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
				animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
				float CurrentAnimationTime = (animationTime % 1)*clip.length;
				TimeFrame.text = "TimeFrame: " + CurrentAnimationTime.ToString("F0") + " Sec";
			}

		}
    }
	

    // Method to check if InstructorAvatar is enabled before performing actions
    private bool IsInstructorAvatarEnabled()
    {
        return InstructorAvatar != null && InstructorAvatar.activeInHierarchy;
    }
	
	public void StartRestart()
    {
		sound.Play();
		InstructorAvatar.SetActive(false);
		InstructorAvatar.SetActive(true);
		
		UserAvatar.SetActive(false);
		UserAvatar.SetActive(true);
		animator.speed = 1f;
		
		ResumeText.SetActive(true);
		PauseText.SetActive(false);
		
		CurrentAnimationSpeed = animator.speed;
		

    }

    // Method to pause or resume the animation
    public void TogglePauseResume()
    {
		sound.Play();
        if (IsInstructorAvatarEnabled() && animator != null)
        {	
            if (isPaused)
            {
                // Resume the animation
                animator.speed = CurrentAnimationSpeed;

				ResumeText.SetActive(true);
				PauseText.SetActive(false);
				
            }
            else
            {
                // Pause the animation
				CurrentAnimationSpeed = animator.speed;
                animator.speed = 0f;
				ResumeText.SetActive(false);
				PauseText.SetActive(true);
				
            }

            // Toggle the paused state
            isPaused = !isPaused;
        }
        else
        {
            Debug.LogWarning("InstructorAvatar is not enabled or Animator is missing!");
        }
    }


    // Method to move the animation forward by 5 seconds
    public void PlayForward()
    {
		
        if (IsInstructorAvatarEnabled() && animator != null && (Time.time - lastClickTime > clickCooldown))
        {
			sound.Play();
			lastClickTime = Time.time;
            // Get the current animation time
            animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

			float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
            // Move the animation forward by 5 seconds (if it's not paused)
            animationTime += 5f/animLength;

            // animationTime = Mathf.Clamp(animationTime, 0f, animLength);
            // Update the animator's time
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animationTime);
			
			Forwarded += 1;
			
        }
        else
        {
            Debug.LogWarning("InstructorAvatar is not enabled or Animator is missing!");
        }
    }

    // Method to move the animation backward by 5 seconds
    public void PlayBackward()
    {
		
        if (IsInstructorAvatarEnabled() && animator != null && (Time.time - lastClickTime > clickCooldown))
        {
			sound.Play();
			lastClickTime = Time.time;
            // Get the current animation time
            animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

			float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
            // Move the animation backward by 5 seconds (if it's not paused)
            animationTime -= 5f/animLength;;

            // Clamp the time to ensure it doesn't go below 0
            animationTime = Mathf.Clamp(animationTime, 0f, 100000f);

            // Update the animator's time
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animationTime);
			
			Forwarded -= 1;
        }
        else
        {
            Debug.LogWarning("InstructorAvatar is not enabled or Animator is missing!");
        }
    }
	
	public void SpeedUp()
    {
		sound.Play();
        if (IsInstructorAvatarEnabled() && animator != null)
        {	
			CurrentAnimationSpeed += 0.1f;
			
			if (isPaused == false)
			{
				animator.speed = CurrentAnimationSpeed;
			}
        }
        else
        {
            Debug.LogWarning("InstructorAvatar is not enabled or Animator is missing!");
        }
    }
	
	public void SlowDown()
    {
		sound.Play();
        if (IsInstructorAvatarEnabled() && animator != null)
        {	
			if (CurrentAnimationSpeed > 0.1f)
			{	
				CurrentAnimationSpeed -= 0.1f;
			}
			
			if (isPaused == false)
			{			
				animator.speed = CurrentAnimationSpeed;
			}
			
        }
        else
        {
            Debug.LogWarning("InstructorAvatar is not enabled or Animator is missing!");
        }
    }
	
	public void SwitchingAvatarSide()
	{	
		sound.Play();
		var NewRotation = InstructorAvatar.transform.eulerAngles;
		InstructorAvatar.transform.rotation = Quaternion.Euler(NewRotation.x, NewRotation.y + 180, NewRotation.z);
		
		var UserAvatarRotation = UserAvatar.transform.eulerAngles;
		UserAvatar.transform.rotation = Quaternion.Euler(UserAvatarRotation.x, UserAvatarRotation.y + 180, UserAvatarRotation.z);
		UserAvatar.transform.localScale = new Vector3(-UserAvatar.transform.localScale.x, UserAvatar.transform.localScale.y, UserAvatar.transform.localScale.z);
	}
	
	
	// public void SwitchingHighlight()
	// {	
		// sound.Play();
		
		// script3.enabled = false;
		// script4.enabled = false;
		// script5.enabled = false;
		// script6.enabled = false;
		// script7.enabled = false;

        // Vector3 position = UserAvatar.transform.position;
        // Quaternion rotation = UserAvatar.transform.rotation;

        // Destroy(UserAvatar);

        // UserAvatar = Instantiate(userAvatarOld, position, rotation);
		
		// UserAvatar.SetActive(false);
		// UserAvatar.SetActive(true);
		
		
		// currentMode = (currentMode + 1) % 5; // Cycles through 0, 1, 2 (Mode1, Mode2, Mode3)
		
		 // switch (currentMode)
        // {
            // case 0:
                // script7.enabled = true;
                // break;
            // case 1:
                // script3.enabled = true;
                // break;
            // case 2:
                // script4.enabled = true;
                // break;
            // case 3:
                // script5.enabled = true;
                // break;
            // case 4:
                // script6.enabled = true;
                // break;
			
        // }
		
	// }
	
}
