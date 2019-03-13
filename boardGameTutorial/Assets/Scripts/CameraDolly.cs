using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour {

    StateManager theStateManager; // Like Game Manager

    public float pivotAngle = 90f;
    private float cameraLockTimer = 1.5f;
    float pivotVelocity;

    // Use this for initialization
    void Start () {
        theStateManager = GameObject.FindObjectOfType<StateManager> ();
    }

    // Update is called once per frame
    void Update () {
        if (PlayerPrefs.GetString ("gameMode") == "singlePlayer") {
            cameraLockTimer -= Time.deltaTime;
            if (cameraLockTimer <= 0) {
                return;
            }
        }

        float theAngle = this.transform.rotation.eulerAngles.y;

        if (theAngle > 180) {
            theAngle -= 360f; // Fixes a bug where the camera rotate a few turns before stopping
        }
        theAngle = Mathf.SmoothDamp (
            theAngle,
            theStateManager.CurrentPlayerId == 0 ? pivotAngle : -pivotAngle,
            ref pivotVelocity,
            0.25f);

        this.transform.rotation = Quaternion.Euler (new Vector3 (0, theAngle, 0));

    }
}