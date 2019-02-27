using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {


    // Use this for initialization
	void Start () {
		
	}

    public Tile[] NextTiles;
    public PlayerStone PlayerStone;
    public bool isScoringSpace;
    public bool isRollAgain;
    public bool isSideline; // Is a part of a player's safe area

	// Update is called once per frame
	void Update () {
		
	}
}
