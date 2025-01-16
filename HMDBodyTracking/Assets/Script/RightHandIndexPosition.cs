using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;

public class RightHandIndexPosition : MonoBehaviour
{
    public OVRHand ovrHand; // The OVRHand component to track hand data
    private OVRSkeleton ovrSkeleton; // The OVRSkeleton component that contains the bones
	public GameObject RightIndexObject;

    void Start()
    {
        // Ensure you have the OVRSkeleton component attached to the same GameObject as OVRHand
        if (ovrHand != null)
        {
            ovrSkeleton = ovrHand.GetComponent<OVRSkeleton>();
        }
    }

    void Update()
    {
        if (ovrHand != null && ovrHand.IsTracked && ovrSkeleton != null)  // Check if the hand is being tracked and OVRSkeleton exists
        {

            Transform indexFingerBone3 = ovrSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index3].Transform;

					
			
			// Move the empty object based on the position and rotation of indexFingerBone3
            if (indexFingerBone3 != null && RightIndexObject != null)
            {
                // Update the empty object's position and rotation to match indexFingerBone3
                RightIndexObject.transform.position = indexFingerBone3.position;
                RightIndexObject.transform.rotation = indexFingerBone3.rotation;
            }

        }
    }
}
