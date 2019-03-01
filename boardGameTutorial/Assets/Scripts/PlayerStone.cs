using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour {

    // Use this for initialization
    void Start () {
        // Find the StateManager Game Object
        theStateManager = GameObject.FindObjectOfType<StateManager> ();

        // The targetPosition is the current position of the stone
        targetPosition = this.transform.position;

        if (!(PlayerPrefs.GetInt ("p1victory") >= 0)) {
            PlayerPrefs.SetInt ("p1victory", 0);
        }

        if (!(PlayerPrefs.GetInt ("p2victory") >= 0)) {
            PlayerPrefs.SetInt ("p2victory", 0);
        }

        // AutoRoll the first roll if the user checked the option
        if (PlayerPrefs.GetString ("AutoRoll") == "yes") {
            GameObject.FindObjectOfType<DiceRoller> ().RollTheDice ();
        }

    }

    public Tile startingTile; // Each stone has a starting tile, depending on the player
    public Tile currentTile { get; protected set; } // And a current tile 

    public int PlayerId; // Really ?
    public StoneStorage MyStoneStorage; // Stone storage of the stone depending on the player
    StateManager theStateManager; // Like Game Manager

    Tile[] moveQueue; // Array of all moves to be done by a stone 
    int moveQueueIndex; // Index of this array

    bool isAnimating = false; // Is the stone animating
    Vector3 targetPosition; // Position of the target we want to reach

    Vector3 velocity = Vector3.zero; // Null vector used for smoothDamp function
    float animationSpeed; // Used to manipulate animation speed through PlayerPrefs
    float smoothTime = 0.25f; // Time of horizontal animation
    float smoothTimeVertical = 0.1f; // Time for vertical animation
    float smoothDistance = 0.01f; // Distance at which point we considered the stone at destination
    float smoothHeight = 0.5f; // Distance to move up before moving sideways

    public bool scored = false; // Stone scored
    bool justGotBopped = false; // Indicate if the stone just got bopped (fixes a bug where you couldn't play after having a stone bopped)
    PlayerStone stoneToBop; // The stone you're going to bop

    public bool justGotClicked = false; // Indicate if the stone has already been clicked (fixes a bug where you could double click your stone)

    // Update is called once per frame
    void Update () {

        // Get the animationSpeed selected by the player
        animationSpeed = PlayerPrefs.GetFloat ("AnimationSpeed");

        if (theStateManager.gamePaused == true) {
            Debug.Log ("PlayerStone Update: Game paused so nothing do");
            return;
        }

        if (isAnimating == false && (theStateManager.playerTwoScore == 6 || theStateManager.playerOneScore == 6)) {
            theStateManager.gameOver = true;
            if (theStateManager.playerOneScore == 6) {
                theStateManager.winner = "PLAYER ONE";
                PlayerPrefs.SetInt ("p1victory", PlayerPrefs.GetInt ("p1victory") + 1);
            } else if (theStateManager.playerTwoScore == 6) {
                theStateManager.winner = "PLAYER TWO";
                PlayerPrefs.SetInt ("p2victory", PlayerPrefs.GetInt ("p2victory") + 1);
            }

            theStateManager.Restart ();
        }

        if (isAnimating == false) {
            //Nothing for us to do if the stone is not animating
            return;
        }

        // If the distance between the stone and the target is inferior to the smoothDistance

        if (Vector3.Distance (
                new Vector3 (this.transform.position.x, targetPosition.y, this.transform.position.z),
                targetPosition) < smoothDistance) {

            // If we scored, and that we're on the scoring tile, we fall down to the scoring box
            if (this.scored == true && this.currentTile != null && this.currentTile.isScoringSpace == true) {
                Rigidbody rb = this.GetComponent<Rigidbody> ();
                rb.isKinematic = false;
                this.currentTile.PlayerStone = null;
                this.currentTile = null;
            }

            // We've reached the (higher than) target position, do we still have moves in the queue ? 
            if ((moveQueue == null || moveQueueIndex == (moveQueue.Length)) && ((this.transform.position.y - smoothDistance) > targetPosition.y)) {
                if (this.scored == false) // If we haven't scored, we have to go down on our tile
                {
                    // We're out of moves (and too high), the only thing left to do is drop down

                    this.transform.position = Vector3.SmoothDamp (
                        this.transform.position,
                        new Vector3 (this.transform.position.x, targetPosition.y, this.transform.position.z),
                        ref velocity,
                        smoothTimeVertical / animationSpeed);

                    if (stoneToBop != null) {
                        // If we have a stone to bop, we send it back to its storage
                        stoneToBop.ReturnToStorage ();
                        stoneToBop = null;
                    }
                }
            } else {
                // Right position, right height, let's advance the queue
                AdvanceMoveQueue ();
            }
        }

        // If the stone is on the board
        else if (this.transform.position.y < (smoothHeight - smoothDistance)) {
            //We want to rise up before moving sideways

            this.transform.position = Vector3.SmoothDamp (
                this.transform.position,
                new Vector3 (this.transform.position.x, smoothHeight, this.transform.position.z),
                ref velocity,
                smoothTimeVertical / animationSpeed);

        }

        // The stone is not on the board and far from the target
        else {
            // Normal movement (sideways)
            // We move to the target position + smoothHeight

            this.transform.position = Vector3.SmoothDamp (
                this.transform.position,
                new Vector3 (targetPosition.x, smoothHeight, targetPosition.z),
                ref velocity,
                smoothTime / animationSpeed);

        }
    }

    void AdvanceMoveQueue () {
        // If there is still moves in the queue
        if (moveQueue != null && moveQueueIndex < moveQueue.Length) {

            Tile nextTile = moveQueue[moveQueueIndex];

            // If the next tile is a scoring space.. we've scored
            if (nextTile.isScoringSpace == true) {
                this.scored = true;

                if (theStateManager.CurrentPlayerId == 0) {
                    theStateManager.playerOneScore++;
                } else {
                    theStateManager.playerTwoScore++;
                }
            }
            // We set the target position to the next tile
            SetNewTargetPosition (nextTile.transform.position);
            moveQueueIndex++;

        }
        // If there is no moves left in the queue
        else {
            if (justGotBopped == false) {
                // The movement queue is empty, so we're done animating
                theStateManager.doneAnimating = true;
                this.isAnimating = false;

                // Are we on a roll again space?
                if (currentTile != null && currentTile.isRollAgain) {
                    theStateManager.RollAgain ();
                }
            }

        }

    }
    void SetNewTargetPosition (Vector3 pos) {
        targetPosition = pos;
        velocity = Vector3.zero;
        isAnimating = true;
    }

    void OnMouseUp () {
        Debug.Log ("Stone clicked");
        MoveMe ();
    }

    public void MoveMe () {
        if (this.justGotClicked == true) {
            Debug.Log ("MoveMe: Stone has just been clicked so returning");
            return;
        }

        // If we just got bopped, we tell the stateManeager we're not done animating ( fixes a bug where you couldn't play after getting a stone bopped)
        if (this.justGotBopped == true) {
            theStateManager.doneAnimating = false;
            this.justGotBopped = false;

        }
        // Is this the correct player ? 
        if (theStateManager.CurrentPlayerId != PlayerId) {
            Debug.Log ("MoveMe: Incorrect player stone so return");
            return;
        }

        if (theStateManager.doneRolling == false) {
            // we can't move until the dice have been thrown
            Debug.Log ("MoveMe: Dice haven't been thrown so return");
            return;
        }
        if (theStateManager.doneAnimating) {
            Debug.Log ("Maybe we're here ??? (return)");
            return;
        }

        int spacesToMove = theStateManager.DiceTotal;

        if (spacesToMove == 0) {
            Debug.Log ("MoveMe: No spaces to move se return");
            return;
        }


        this.justGotClicked = true;

        // Where should we end up ? 
        moveQueue = GetTilesAhead (spacesToMove);
        Tile finalTile = moveQueue[moveQueue.Length - 1];

        if (finalTile != null) {

            if (!CanLegallyMoveTo (finalTile)) {
                // Not allowed
                finalTile = currentTile;
                moveQueue = null;
                Debug.Log ("MoveMe: Can't move to final tile");
                return;
            }

            // If there is an enemy stone in our legal space, we kick it out 
            if (finalTile.PlayerStone != null) {
                stoneToBop = finalTile.PlayerStone;
                stoneToBop.justGotBopped = true;
                stoneToBop.currentTile.PlayerStone = null;
                stoneToBop.currentTile = null;
            }
        }

        this.transform.SetParent (null); // Become Batman

        // Remove ourself from our old tile
        if (currentTile != null) {
            currentTile.PlayerStone = null;
        }

        if (this.scored == false) {
            // Put ourselves in our new tile
            finalTile.PlayerStone = this;
        }

        moveQueueIndex = 0;
        currentTile = finalTile;

        theStateManager.doneClicking = true;
        this.isAnimating = true;
    }

    // Return the list of tiles X moves ahead of us
    public Tile[] GetTilesAhead (int spacesToMove) {
        if (spacesToMove == 0) {
            return null;
        }

        Tile[] listOfTiles = new Tile[spacesToMove];
        Tile finalTile = currentTile;

        for (int i = 0; i < spacesToMove; i++) {
            if (finalTile == null) {
                finalTile = startingTile;
            } else {
                if (finalTile.NextTiles == null || finalTile.NextTiles.Length == 0) {
                    // We are overshooting the victory, so just return so nulls in the array
                    // Just break and we'll return the array with nulls at the end
                    break;
                } else if (finalTile.NextTiles.Length > 1) {
                    // Branch based on playerID
                    finalTile = finalTile.NextTiles[PlayerId];
                } else {
                    finalTile = finalTile.NextTiles[0];
                }
            }
            listOfTiles[i] = finalTile;
        }
        return listOfTiles;
    }

    public Tile GetTileAhead () {
        return GetTileAhead (theStateManager.DiceTotal);

    }

    // Return the final tile we'd land on if we moved __ spaces
    public Tile GetTileAhead (int spacesToMove) {
        Tile[] tiles = GetTilesAhead (spacesToMove);

        if (tiles == null) {
            // We aren't moving at all, so just return our current tile ? 
            return currentTile;
        }

        return tiles[tiles.Length - 1];

    }

    public bool CanLegallyMoveAhead (int spacesToMove) {
        Tile theTile = GetTileAhead (spacesToMove);
        return CanLegallyMoveTo (theTile);
    }

    bool CanLegallyMoveTo (Tile destinationTile) {
        if (destinationTile == null) {
            // NOTE: A null tile means we are overshooting the victory roll
            return false;
        }
        // Is the tile empty ?
        if (destinationTile.PlayerStone == null) {
            Debug.Log ("Stone " + this.gameObject.name + " Can move to : " + destinationTile.gameObject.name);
            return true;
        }
        // Is it one of our stone ? 
        if (destinationTile.PlayerStone.PlayerId == this.PlayerId) {
            // We can't land on our stones
            return false;
        }

        // If it's an enemy stone, is it in a safe square?
        if (destinationTile.isRollAgain) {
            // Can't bop someone on a safe tile
            return false;
        }

        return true;
    }

    public void ReturnToStorage () {
        moveQueue = null;
        //Save our current position
        Vector3 savePosition = this.transform.position;
        MyStoneStorage.AddStoneToStorage (this.gameObject);

        // Set our new positio nto the animation target
        SetNewTargetPosition (this.transform.position);

        // restore our saved position
        this.transform.position = savePosition;

    }

}