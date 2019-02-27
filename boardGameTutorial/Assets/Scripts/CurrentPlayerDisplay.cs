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
        p1victory = GameObject.Find("P1Victory").GetComponent<Text>();
        p2victory = GameObject.Find("P2Victory").GetComponent<Text>();
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
        p1victory.text = " P1 : " + PlayerPrefs.GetInt("p1victory");
        p2victory.text = " P2 : " + PlayerPrefs.GetInt("p2victory");

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
