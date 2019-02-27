using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour {

	// Use this for initialization
	void Start () {
        theStateManager = GameObject.FindObjectOfType<StateManager>(); 

	}
    StateManager theStateManager;   // Like Game Manager

    public float pivotAngle = 90f;
    float pivotVelocity;
	
	// Update is called once per frame
	void Update () {

        float theAngle = this.transform.rotation.eulerAngles.y;
        if (theAngle > 180)
        {
            theAngle -= 360f;       // Fixes a bug where the camera didn't stop switching
        }
        theAngle= Mathf.SmoothDamp(
            theAngle,
            theStateManager.CurrentPlayerId==0? pivotAngle : -pivotAngle,
            ref pivotVelocity,
            0.25f);


        this.transform.rotation = Quaternion.Euler( new Vector3(0,theAngle,0));
	}
}
