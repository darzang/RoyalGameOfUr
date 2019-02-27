using UnityEngine;
using System.Collections.Generic;
public class AIPlayer
{
    public AIPlayer()
    {
        stateManager = GameObject.FindObjectOfType<StateManager>();
    }

    StateManager stateManager;

    virtual public void DoAI()  // Virtual fonction??? --> this function can be overwritten by a child class
    {

        if (stateManager.doneRolling == false)
        {
            // We roll the dice
            DoRoll();
            return;
        }

        if (stateManager.doneClicking == false)
        {
            // We rolled the dice but we need to pick a stone
            DoClick();
            return;
        }


    }

    virtual protected void DoRoll()
    {
        GameObject.FindObjectOfType<DiceRoller>().RollTheDice();
    }

    virtual protected void DoClick()
    {
        // Pick a stone to move then "click" it


        PlayerStone[] legalStones = GetLegalMoves();

        if (legalStones == null || legalStones.Length == 0)
        {
            // We have no legal moves, we might still be in a delayed coroutine somewhere
            return;
        }

        // BasicAI simply picks a legal move at random
        PlayerStone pickedStone = PickStoneToMove(legalStones);
        
        pickedStone.MoveMe();

    }
    virtual protected PlayerStone PickStoneToMove(PlayerStone[] legalStones)
    {
        return legalStones[Random.Range(0, legalStones.Length)];
    }

    // Returns a list of stones that can legally moved
    protected PlayerStone[] GetLegalMoves()
    {

        List<PlayerStone> legalStones = new List<PlayerStone>();

        // If we rolled a zero, we have no legal moves
        if (stateManager.DiceTotal == 0)
        {
            return legalStones.ToArray();
        }

        // Loop through all of a player's stones
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();  // Retrieve the stones of the two players

        foreach (PlayerStone ps in pss)
        {
            if (ps.PlayerId == stateManager.CurrentPlayerId)
            {
                if (ps.CanLegallyMoveAhead(stateManager.DiceTotal) && ps.scored == false)
                {
                    legalStones.Add(ps);
                }
            }
        }
        return legalStones.ToArray();

    }
}
