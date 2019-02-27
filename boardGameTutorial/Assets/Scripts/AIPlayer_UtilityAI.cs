using System.Collections.Generic;
using UnityEngine;

public class AIPlayer_UtilityAI : AIPlayer
{

    // TOIMPROVE : it's good to stay on the safe tile, but mostly the one in the middle, not that much on our side
    // TODO : Add goodness for tiles ahead of our enemies and then likely to get boped
    // TODO : Add goodness for moving a stone forwards when blocking friendly stones

    Dictionary<Tile,float> tileDanger;
    override protected PlayerStone PickStoneToMove(PlayerStone[] legalStones)
    {
       // float aggressivenessBonus = 1;

        if (legalStones == null || legalStones.Length == 0)
        {
            // We're trying to pick a Stone be we have none? 
            return null;
        }

        CalculateTileDanger(legalStones[0].PlayerId);
        // For each stone, we rank how could it would be to pick it (0..1)
        PlayerStone bestStone=null;
        float goodness = 0;

        foreach (PlayerStone ps in legalStones)
        {
            float g = GetStoneGoodness(ps, ps.currentTile, ps.GetTileAhead());
            if (bestStone == null || g > goodness)
            {
                bestStone = ps;
                goodness = g;
            }
        }
        return bestStone;
        //return base.PickStoneToMove(legalStones);
    }


    virtual protected void CalculateTileDanger(int myPlayerId)
    {
        tileDanger = new Dictionary<Tile, float>();
        Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

        foreach (Tile t in tiles)
        {
            tileDanger[t] = 0;
        }

        PlayerStone[] allStones = GameObject.FindObjectsOfType<PlayerStone>();

        foreach (PlayerStone stone in allStones)
        {
            if (stone.PlayerId == myPlayerId)
            {
                continue;           // Skip to the next iteration of the loop 
            }

            // If this is an enemy stone, add a danger value to tiles in front of it (unless safe)

            for (int i = 1; i <=4; i++)
            {
                Tile t = stone.GetTileAhead(i);

                if (t == null)
                {
                    // This tile (and subsequent tiles) are invalid, so we can bail
                    break;
                }
                if (t == null || t.isScoringSpace || t.isSideline || t.isRollAgain)
                {
                    // This tile is not a danger zone, so we can ignore it
                    continue;
                }

                // Okay, This tile is within bopping range of an enemy, so it's dangerous
               if(i==2)
               {
                   // 2 is most likely so most dangerous
                   tileDanger[t]+=0.4f;
               }
                else if(i==1 || i ==3)
               {
                    tileDanger[t] += 0.2f;
                }
               else
               {
                   tileDanger[t] += 0.1f;
               }

            }

        }
    }

    virtual protected float GetStoneGoodness(PlayerStone stone, Tile currentTile, Tile futureTile)
    {
        float goodness = 0; // not 0 so that it doesn't pick the first one in case of equality

        if (currentTile == null)
        {
            // We aren't on the board yet and it's always nice to add stones to the board
            goodness += 0.2f;
        }
        if (currentTile != null &&  (currentTile.isRollAgain == true && currentTile.isSideline ==false))
        {
            // We are on a roll again tile in the middle, let's stay to block the enemy
            goodness -= 0.3f;
        }

        if (futureTile.isRollAgain == true)
        {
            goodness += 0.5f;
        }

        if (futureTile.isSideline == true)
        {
            goodness += 0.2f;
        }

        if (futureTile.PlayerStone != null && futureTile.PlayerStone.PlayerId != stone.PlayerId)
        {
            // There's an enemy stone to bop!
            goodness += 0.8f; 
        }
        if (futureTile.isScoringSpace)
        {
            goodness += 0.2f;
        }


        float currentDanger = 0;

        if (currentTile != null)
        {
            currentDanger = tileDanger[currentTile];
        }

        goodness += currentDanger -tileDanger[futureTile];  // If the future tile is more dangerous, goodness goes down




        return goodness; 
    }
    
}

