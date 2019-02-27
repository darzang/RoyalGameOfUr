using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneStorage : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// Create one stone for each placeholder 
        for (int i = 0; i < this.transform.childCount; i++)
        {
            // Instantiate a new copy of the stone prefab
            GameObject theStone = Instantiate(StonePrefab);
            theStone.GetComponent<PlayerStone>().startingTile = this.StartingTile;
            theStone.GetComponent<PlayerStone>().MyStoneStorage = this;

            AddStoneToStorage(theStone, this.transform.GetChild(i));
        }
	}

    public GameObject StonePrefab;
	public Tile StartingTile;

    public void AddStoneToStorage(GameObject theStone, Transform thePlaceHolder=null)
    {
        if (thePlaceHolder == null) // child of stone storage
        {

            // Find the first empty placeholder
            for (int i = 0; i < this.transform.childCount; i++)
            {   
                Transform p = this.transform.GetChild(i);       // p = placeholder
                if (p.childCount == 0)
                {
                    // This placeholder is empty
                    thePlaceHolder = p;
                    break;
                }
            }

            if (thePlaceHolder == null)
            {
                Debug.LogError("Trying to add a stone but no placeholder empty, how did you do that ?!");
                return;
            }
        }

        // Parent the stone to the placeholder 
        theStone.transform.SetParent(thePlaceHolder);

        // Reset the stone's local position to 0,0,0 inside the stone storage
        theStone.transform.localPosition = Vector3.zero;

    }
}
