using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour {
    // TODO : 3 AI 
    // TODO : language option
    // TODO : Create a cool intro ? 
    // TODO : cool animation for dices 
    // TODO : little text appearing in the bottom telling us the change has been done in options
    // TODO : Highlighting on options button in the pause menu 

    void Awake () {
        DontDestroyOnLoad (this);
    }

    // Use this for initialization
    void Start () {

        PlayerAIs = new AIPlayer[NumbersOfPlayers];

        if (PlayerPrefs.GetString ("gameMode") == "multiPlayer") {
            PlayerAIs[0] = null;
            PlayerAIs[1] = null;
            Debug.Log ("Two human players");

        } else {
            Debug.Log ("One Human one AI");
            switch (PlayerPrefs.GetString ("difficulty")) {
                case "easy":
                    PlayerAIs[0] = null;
                    PlayerAIs[1] = new AIPlayer ();
                    break;
                case "medium":
                    PlayerAIs[0] = null;
                    PlayerAIs[1] = new AIPlayer_UtilityAI ();
                    break;
                case "hard":
                    PlayerAIs[0] = null;
                    PlayerAIs[1] = new AIPlayer_UtilityAI ();
                    break;
                default:
                    Debug.Log ("Couldn't find difficulty");
                    break;

            }
        }

        UICanvas.enabled = true;
        PauseCanvas.enabled = false;

    }
    public Canvas UICanvas;
    public Canvas PauseCanvas;
    public Canvas GameOverCanvas;
    public TextMeshProUGUI VictoryText;

    public bool gameOver = false;
    public string winner = "NO ONE";
    public int playerOneScore = 0;
    public int playerTwoScore = 0;
    public int playerOneVictory = 0;
    public int playerTwoVictory = 0;
    public int NumbersOfPlayers = 2;
    public int CurrentPlayerId = 0;

    public bool gamePaused = false;

    AIPlayer[] PlayerAIs;
    public int DiceTotal;

    public bool doneRolling = false;
    public bool doneClicking = false;
    public bool doneAnimating = false;

    public GameObject NoLegalMovesPopup; // Text displayed when no moves are possible

    public void Paused () {
        gamePaused = true;
        UICanvas.gameObject.SetActive (false);
        PauseCanvas.gameObject.SetActive (true);
    }

    public void Resume () {
        gamePaused = false;
        UICanvas.gameObject.SetActive (true);
        PauseCanvas.gameObject.SetActive (false);
    }

    public void ReturnToMenu () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex - 1);
    }

    public void Restart () {
        SceneManager.LoadScene ("Play");
    }

    public void NewTurn () {
        // Set the justGotClicked boolean of all stones to false 
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone> (); // Retrieve the stones of the two players
        foreach (PlayerStone ps in pss) {
            ps.justGotClicked = false;
        }

        // This is the start of a player's turn 
        doneRolling = false;
        doneClicking = false;
        doneAnimating = false;

        if (PlayerPrefs.GetString ("AutoRoll") == "yes") {
            GameObject.FindObjectOfType<DiceRoller> ().RollTheDice ();
        }

        CurrentPlayerId = (CurrentPlayerId + 1) % NumbersOfPlayers; // Either 0 or 1 
    }

    public void RollAgain () {
        // Set the justGotClicked boolean of all stones to false 
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone> (); // Retrieve the stones of the two players
        foreach (PlayerStone ps in pss) {
            ps.justGotClicked = false;
        }

        doneRolling = false;
        doneClicking = false;
        doneAnimating = false;

        if (PlayerPrefs.GetString ("AutoRoll") == "yes") {
            GameObject.FindObjectOfType<DiceRoller> ().RollTheDice ();
        }

    }

    // Update is called once per frame
    void Update () {

        if (gameOver == true) {
            UICanvas.gameObject.SetActive (false);
            GameOverCanvas.gameObject.SetActive (true);
        }

        if (gamePaused == true) {
            return;
        }
        // Is the turn done? 
        if (doneAnimating && doneClicking && doneRolling) {
            NewTurn ();
            return;
        }

        if (PlayerAIs[CurrentPlayerId] != null) {
            PlayerAIs[CurrentPlayerId].DoAI ();
        }

    }

    public void CheckLegalMoves () {
        // If we rolled a zero, we have no legal moves
        if (DiceTotal == 0) {
            StartCoroutine (NoLegalMove ());
            return;
        }

        // Loop through all of a player's stones

        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone> (); // Retrieve the stones of the two players

        bool hasLegalMoves = false;

        foreach (PlayerStone ps in pss) {
            if (ps.PlayerId == CurrentPlayerId) {
                if (ps.CanLegallyMoveAhead (DiceTotal) && ps.scored == false) {
                    // TODO: Highlight stones that can be legally moved
                    hasLegalMoves = true;
                }
            }
        }

        // If no legal moves, wait a sec then advance player
        if (!hasLegalMoves) {
            StartCoroutine (NoLegalMove ());
            return;
        }
    }

    IEnumerator NoLegalMove () {
        // Display message
        NoLegalMovesPopup.SetActive (true);
        // Wait 1 sec
        yield return new WaitForSeconds (0.001f);
        NoLegalMovesPopup.SetActive (false);

        NewTurn ();
    }
}