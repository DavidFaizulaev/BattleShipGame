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
    private GameObject appwarp_logic;
    private SC_Logic appwarp_logic_sc;
    private Button[] all_buttons;
    private bool first_Completion;
    private int hit_Counter;
    private int curr_ship_indx;

    public Text turn_msg;

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

        appwarp_logic = GameObject.Find("NetworkManager");
        appwarp_logic_sc = appwarp_logic.GetComponent<SC_Logic>();

        if (appwarp_logic_sc.IsItMine())
            turn_msg.text = "Your turn - Place 2 battleships on your game board\n \t\tFirst of 4 squars \n\t\tSecond of 3 squars";

        else
            turn_msg.text = "Opponent turn - Places 2 battleships on their game board";

        Debug.Log("done start in pbm");
    }

    void Update()
    {
        if (appwarp_logic_sc.updateboards)
        {
            if (appwarp_logic_sc.game_result != (-9999))
            {
                //you won the game
                if (appwarp_logic_sc.game_result == 1)
                    ConnStater.set_Game_Result(true);

                else
                    ConnStater.set_Game_Result(false);
            }
            else
            {
                EnemyMove(appwarp_logic_sc.enemy_move);
                appwarp_logic_sc.updateboards = false;
            }
        }

        if((first_Completion)&&(appwarp_logic_sc.IsItMine()))
            turn_msg.text = "Your turn - try to attack the enemies ships";

        if ((first_Completion) && (!appwarp_logic_sc.IsItMine()))
            turn_msg.text = "Opponent turn - will now try attack your ships";
    }

    public void OnButtonPressed(Button btn)
    {
        Vector2 btnPos = btn.GetComponent<ButtonInfo>().position;
        Debug.Log("vector x " + btnPos.x + "vector y " + btnPos.y);
        //checking if boat structure is not complete yet and if it's the player's turn to build ship

        if ((appwarp_logic_sc.IsItMine()) && (CheckStructure() == false))
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
            first_Completion = true;
            //message code - 2 - battleship creation complete - switch turn to other player.
            turn_msg.text = "Your turn has ended - now changing turn to opponent";
            appwarp_logic_sc.MakeMyMove("2");
            Debug.Log("structure complete - change turn to other player");
        }
    }

    //Accessed by EnemyAI in order to try and hit a player's battlesip.
    public void EnemyMove(string str)
    {
        //bool attck_result = allPlayerShips[0].ifexists(vc);
        //if 'attck_result' is TRUE then the vector recieved marks a ship's location
        //if (attck_result)

        Vector2 vc;

        int startInd = str.IndexOf("X:") + 2;
        float aXPosition = float.Parse(str.Substring(startInd, str.IndexOf(" Y") - startInd));
        startInd = str.IndexOf("Y:") + 2;
        float aYPosition = float.Parse(str.Substring(startInd, str.IndexOf("}") - startInd));

        vc = new Vector2(aXPosition, aYPosition);

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
                      //  Turn.set_LastValidAttackCrods(vc);
                        hit_Counter++;
                        appwarp_logic_sc.MakeMyMove("1");

                        //if (hit_Counter == SgameInfo.max_Ship_Size)
                        if (hit_Counter == SgameInfo.max_number_of_hits)
                        {
                           Debug.Log("game over - AI won");
                           appwarp_logic_sc.MakeMyMove("I won the game");
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
                        appwarp_logic_sc.MakeMyMove("0");
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

    
}