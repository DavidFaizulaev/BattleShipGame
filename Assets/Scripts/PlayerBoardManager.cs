//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Operation of the player game board.
//Allows the player to place the battleship and is used by the enemy AI in order to make a game move.
//either hitting or missing the player's battleship.

public class PlayerBoardManager : MonoBehaviour
{
    private BattleShip[] allPlayerShips;
    private Button[] all_buttons;
    private bool first_Completion;
    private int hit_Counter;
    private int curr_ship_indx;

    void Start()
    {
        hit_Counter = 0;
        first_Completion = false;
        curr_ship_indx = 0;

        allPlayerShips = new BattleShip[SgameInfo.max_number_of_ships];

        for (int i = 0; i < SgameInfo.max_number_of_ships; i++)
        {
           allPlayerShips[i] = new BattleShip();
           allPlayerShips[i].Init_Ship(SgameInfo.max_Ship_Size-i);
        }
        
        //Debug.Log("done start in pbm");
    }

    public void OnButtonPressed(Button btn)
    {
        Vector2 btnPos = btn.GetComponent<ButtonInfo>().position;
        //Debug.Log("vector x " + btnPos.x + "vector y " + btnPos.y);
        //checking if boat structure is not complete yet and if it's the player's turn to build ship

        if ((TurnManager.Pturn) && (CheckStructure() == false))
        {
            if (allPlayerShips[curr_ship_indx].Set_Loc(btnPos))
            {
                //location is available and ship can be placed
                btn.image.color = new Color(Color.green.r, Color.green.g, Color.green.b, 1f);
            }
        }

        if ((curr_ship_indx != (SgameInfo.max_number_of_ships-1)) && (CheckStructure()))
            curr_ship_indx++;

        //checking if boat structure is complete and transfer the turn to the AI.
        if ((first_Completion == false) && (CheckStructure()))
        {
            TurnManager.EndTurn(false, true);
            first_Completion = true;
            //Debug.Log("structure complete - change turn to PC");
        }
    }

    //Accessed by EnemyAI in order to try and hit a player's battlesip.
    public void EnemyMove(Vector2 vc)
    {
        //bool attck_result = allPlayerShips[0].ifexists(vc);
        //if 'attck_result' is TRUE then the vector recieved marks a ship's location
        //if (attck_result)
        if (GetAttackResult(vc) != -1) 
        {
            //move successful
            //ship exits
            //ship loc marked as hit
          //  Debug.Log("attack success");
            all_buttons = this.GetComponentsInChildren<Button>();
            foreach (Button b in all_buttons)
            {
                if ((b.GetComponent<ButtonInfo>().position.x == vc.x) &&
               (b.GetComponent<ButtonInfo>().position.y == vc.y))
                {
                    //checking if location has been already marked as missed
                    if (b.image.color.Equals(Color.black) == false)
                    {
                        //mark location as hit
                        b.image.color = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
                      //  Turn.set_LastValidAttackCrods(vc);
                        hit_Counter++;

                        //if (hit_Counter == SgameInfo.max_Ship_Size)
                        if (hit_Counter == SgameInfo.max_number_of_hits)
                        {
                            TurnManager.AIwon = true;
                            //Debug.Log("game over - AI won");
                            //winner code - 2 - AI won
                            TurnManager.RestartLevel(2);
                        }
                    }
                }
            }
        }
        //'attck_result' is FALSE the vector recieved does not mark a ship's location
        else
        {
            //get button loc from vector and color grey - no ship
           // Debug.Log("attack failed");
            all_buttons = this.GetComponentsInChildren<Button>();
            foreach (Button b in all_buttons)
            {
                if ((b.GetComponent<ButtonInfo>().position.x == vc.x) &&
                   (b.GetComponent<ButtonInfo>().position.y == vc.y))
                {
                    //checking if location has been already marked as HIT or ship loc marked
                    if ((b.image.color.Equals(Color.red) == false) && (b.image.color.Equals(Color.green) == false))
                    {
                        //mark location as missed attempt
                        b.image.color = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
                    }
                }
            }
        }
    }

    public void PlayerMove()
    {
       // Debug.Log("player's turn now");
    }

    private bool CheckStructure()
    {
        return (allPlayerShips[curr_ship_indx].Getstructure_state());
    }

    private int GetAttackResult(Vector2 atck_pos)
    {
        bool attck_result = false;
        int ship_indx = (-1);

        for (int i = 0; i < SgameInfo.max_number_of_ships; i++)
        {
            attck_result = allPlayerShips[i].ifexists(atck_pos);

            if (attck_result)
            {
                ship_indx = i;
                break;
            }
        }
        return ship_indx;
    }
}