using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
        playerText = GetComponent<Text>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
    }

    Text playerText;
    Text scoreText;
    Text p1victory;
    Text p2victory;
    StateManager theStateManager;

    string[] numberWords = {"White","Black"};

	// Update is called once per frame
	void Update () {

		playerText.text = "Player: " + numberWords[theStateManager.CurrentPlayerId];

        if (theStateManager.CurrentPlayerId == 0)
        {
            scoreText.text = "Score: " + theStateManager.playerOneScore.ToString();
        }
        else
        {
            scoreText.text = "Score: " + theStateManager.playerTwoScore.ToString();
        }
	}
}
