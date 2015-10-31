//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Operation of the player game board.
//Allows the player to place the battleship and is used by the opponent in order to make a game move.
//either hitting or missing the player's battleship.

public class MultiplayerBoardManager : MonoBehaviour
{
    private BattleShip[] allPlayerShips;
 //   private GameObject appwarp_logic;
    public SC_Logic appwarp_logic_sc;
    private Button[] all_buttons;
    private bool first_Completion;
    private int hit_Counter;
    private int curr_ship_indx;

    public bool struct_state;

    public Text turn_msg;

    void Start()
    {
        hit_Counter = 0;
        first_Completion = false;
        curr_ship_indx = 0;
        struct_state = false;
        allPlayerShips = new BattleShip[SgameInfo.max_number_of_ships];

        for (int i = 0; i < SgameInfo.max_number_of_ships; i++)
        {
           allPlayerShips[i] = new BattleShip();
           allPlayerShips[i].Init_Ship(SgameInfo.max_Ship_Size-i);
        }

      //  appwarp_logic = GameObject.Find("NetworkManager");
   //     appwarp_logic_sc = appwarp_logic.GetComponent<SC_Logic>();

        if (appwarp_logic_sc.IsItMine())
            turn_msg.text = "Your turn - Place " + SgameInfo.max_number_of_ships +" battleships on your game board\n \t\tFirst of 4 squars \n\t\tSecond of 3 squars and so on";

        else
            turn_msg.text = "Opponent turn - Places " + SgameInfo.max_number_of_ships + " battleships on their game board";

        Debug.Log("done start in pbm");
    }

    public void OnButtonPressed(Button btn)
    {
        Vector2 btnPos = btn.GetComponent<ButtonInfo>().position;
        Debug.Log("vector x " + btnPos.x + "vector y " + btnPos.y);
        //checking if boat structure is not complete yet and if it's the player's turn to build ship

        struct_state = CheckStructure();

        if ((appwarp_logic_sc.IsItMine()) && (struct_state == false))
        {
            if (allPlayerShips[curr_ship_indx].Set_Loc(btnPos))
            {
                //location is available and ship can be placed
                btn.image.color = new Color(Color.green.r, Color.green.g, Color.green.b, 1f);
            }
        }

        if ((curr_ship_indx != (SgameInfo.max_number_of_ships - 1)) && (struct_state))
            curr_ship_indx++;

        //checking if boat structure is complete and transfer the turn to the AI.
        if ((first_Completion == false) && (struct_state))
        {
            first_Completion = true;
            //message code - 2 - battleship creation complete - switch turn to other player.
            turn_msg.text = "Your turn has ended - now changing turn to opponent, to place battleships on board";
            Debug.Log("structure complete - change turn to other player");
            appwarp_logic_sc.MakeMyMove("structure complete");
        }
    }

    //Accessed by EnemyAI in order to try and hit a player's battlesip.
    public void EnemyMove(Vector2 vc)
    {

        Debug.Log("handeling enemy move");

        //if 'GetAttackResult' is TRUE then the vector recieved marks a ship's location
        if (GetAttackResult(vc) != -1) 
        {
            //move successful
            //ship exits
            //ship loc marked as hit
            //Debug.Log("attack success");
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
                        hit_Counter++;
                        appwarp_logic_sc.MakeMyMove("AttackResultSuccess");

                        if (hit_Counter == SgameInfo.max_number_of_hits)
                        {
                           Debug.Log("game over - opponent won");
                           ConnStater.set_Game_Result(0);
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
                        appwarp_logic_sc.MakeMyMove("AttackResultMiss");
                    }
                }
            }
        }
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

    public bool ShipsPlaced()
    {
        return first_Completion;
    }
}