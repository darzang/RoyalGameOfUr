using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour {
	
	public int[] DiceValues;


    public Sprite[] DiceImageOne;
    public Sprite[] DiceImageZero;

	// Use this for initialization
	void Start () {
		DiceValues = new int[4];
        theStateManager = GameObject.FindObjectOfType<StateManager>();
	}
    StateManager theStateManager;
	// Update is called once per frame
	void Update () {
		
	}

  

	public void RollTheDice() {

        if (theStateManager.doneRolling)
        {
            // We've already rolled this turn
            return;
        }
        // In Ur, you roll four dice (tetrahedron), which have half their faces with a value of 1 and half with a value of 0

		// We are going to use random number generation instead of rolling actual physics enabled dice

        theStateManager.doneRolling = false;
        theStateManager.DiceTotal = 0;

		for (int i=0; i<DiceValues.Length; i++){


			DiceValues[i] = Random.Range(0,2);          // 0..2 to get 0 and 1 values
            theStateManager.DiceTotal += DiceValues[i];                   

            // Update the visual to show the dice roll
            if(DiceValues[i]==0){
                this.transform.GetChild(i).GetComponent<Image>().sprite =   // The image of the die we have to change
                    DiceImageZero[Random.Range(0, DiceImageZero.Length)];   // randomly generated between the ones availables
            }
            else
            {
                this.transform.GetChild(i).GetComponent<Image>().sprite =
                    DiceImageOne[Random.Range(0, DiceImageOne.Length)];
            }

		}
        theStateManager.doneRolling = true;
        theStateManager.CheckLegalMoves();

	}
}
