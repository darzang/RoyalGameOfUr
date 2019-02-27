using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTotalDisplay : MonoBehaviour {

    StateManager theStateManager;
	// Use this for initialization
	void Start () {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!theStateManager.doneRolling)
        {
            GetComponent<Text>().text = "= ?" ;
        }
        else
        {
            GetComponent<Text>().text = "= " + theStateManager.DiceTotal;
        }
	}
}
