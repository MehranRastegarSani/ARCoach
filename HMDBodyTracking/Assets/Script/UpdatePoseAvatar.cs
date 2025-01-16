using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePoseAvatar : MonoBehaviour
{
	public GameObject InstructorAvatar;
	public GameObject InstructorAvatar_R_Shoulder;
	public GameObject InstructorAvatar_L_Shoulder;
	public GameObject UserAvatar_R_Shoulder;
	public GameObject UserAvatar_L_Shoulder;
	// private Transform NewTransform;
	

    // Update is called once per frame
    void Update()
    {

		// GameObject InstructorAvatar_R_Shoulder = GameObject.FindWithTag("InstructorAvatar_R_Shoulder");
		// GameObject UserAvatar_R_Shoulder = GameObject.FindWithTag("UserAvatar_R_Shoulder");
		
		
		// GameObject InstructorAvatar_L_Shoulder = GameObject.FindWithTag("InstructorAvatar_L_Shoulder");
		// GameObject UserAvatar_L_Shoulder = GameObject.FindWithTag("UserAvatar_L_Shoulder");

		
		if (transform.localScale.x > 0)
		{
			UserAvatar_R_Shoulder.transform.position = InstructorAvatar_R_Shoulder.transform.position;
			UserAvatar_L_Shoulder.transform.position = InstructorAvatar_L_Shoulder.transform.position;
		}
		else
		{
			UserAvatar_R_Shoulder.transform.position = InstructorAvatar_L_Shoulder.transform.position;
			UserAvatar_L_Shoulder.transform.position = InstructorAvatar_R_Shoulder.transform.position;
		}
		
		
		// Vector3 NewPosition = InstructorAvatar.transform.position;
		// Debug.Log(NewPosition);
		// transform.position = new Vector3(NewPosition.x, transform.position.y, NewPosition.z);
		
		// var NewRotation = cameraRig.centerEyeAnchor.eulerAngles;
		// transform.rotation = Quaternion.Euler(0, NewRotation.y + 270, 270);

		// Debug.Log(NewRotation);

		// transform.localRotation = Quaternion.Euler(NewRotation.x + 180, NewRotation.y + 270, NewRotation.z);
		// transform.eulerAngles = new Vector3(NewRotation.x + 180, NewRotation.y + 270, NewRotation.z);
    }
}
