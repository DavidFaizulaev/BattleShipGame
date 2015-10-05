//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Operation of the AI game board.
//At initialization the AI places the battleship and is used by the PlayerBoardManager
//in order to make a game move upon button press.
//either hitting or missing the AI's battleship.
public class EnemyBoardManager : MonoBehaviour {

	public EnemyAI enemyAI;
    private BattleShip[] allAIships;
	private Button[] all_buttons;
	private int hit_Counter;

	void Start ()
	{
		Debug.Log ("start in ebm");
		hit_Counter = 0;

        allAIships = new BattleShip[SgameInfo.max_number_of_ships];

        for (int i = 0; i < SgameInfo.max_number_of_ships; i++)
        {
            allAIships[i] = new BattleShip();
            allAIships[i].Init_Ship(SgameInfo.max_Ship_Size - i);
            place_Battleship(allAIships[i], SgameInfo.max_Ship_Size - i);
            Debug.Log("ship sized created  " + (SgameInfo.max_Ship_Size - i));
        }

        Debug.Log ("done start in ebm");
	}

    private void place_Battleship(BattleShip btshp,int ship_Size)
	{
		Vector2 vc;

		int random_x_value=0;
		int random_y_value=0;

		//int last_placed_x = 0;
		//int last_placed_y = 0;

		int initial_y = 0;
		int initial_x = 0;

		int i=0;
		int temp_calc;

		//placing first loc of AI ship
		random_y_value = Random.Range (0, 10);
		random_x_value = Random.Range (0, 10);

		vc = new Vector2 (random_x_value, random_y_value);

        if (btshp.Set_Loc(vc)) 
		{
			//location ok - increase counter
			initial_y = random_y_value;
			initial_x = random_x_value;
			//last_placed_y = initial_y;
	//		last_placed_x = random_x_value;
		//	Debug.Log("Placed AI ship at X" + vc.x + " Y" + vc.y);
			i++;
		}

		//Debug.Log ("continuing creation of AI's ship");

		//placing ship with 4 squars
		while (i<ship_Size)
		{
			// generating location for ship
			// if condition occurs, then Y axis is contant and X values will be randomly generated.
			if (initial_y>initial_x)
			{
				temp_calc = initial_x - (ship_Size - (ship_Size - i));
				do {
					random_x_value = Random.Range (temp_calc, ((initial_x+(ship_Size-1))-(i-1)));
				} while ((random_x_value>=10)||(random_x_value<0));

				vc = new Vector2 (random_x_value, initial_y);

                if (btshp.Set_Loc(vc)) 
				{
					//location ok - increase counter
					//last_placed_x = random_x_value;
			//		Debug.Log("Placed AI ship at X" + vc.x + " Y" + vc.y);
					i++;
				}
			}
			//X axis is contant and Y values will be randomly generated.
			else
			{
				temp_calc = initial_y - (ship_Size - (ship_Size - i));
				do {
						random_y_value = Random.Range (temp_calc, ((initial_y+(ship_Size-1))-(i-1)));
				} while ((random_y_value>=10)||(random_y_value<0));

				vc = new Vector2 (initial_x, random_y_value);

                if (btshp.Set_Loc(vc)) 
				{
					//location ok - increase counter
					//last_placed_y = random_y_value;
				//	Debug.Log("Placed AI ship at X" + vc.x + " Y" + vc.y);
					i++;
				}
			}
		}
	}
    
    public void OnButtonPressed(Button btn)
    {
		Debug.Log ("in OnButtonPressed ebm");

        if (TurnManager.Pturn)
        {

			Vector2 btnPos = btn.GetComponent<ButtonInfo> ().position;

			Debug.Log ("vector x " + btnPos.x + "vector y " + btnPos.y);
			//checking if boat structure is not complete yet and if it's the player's turn to attack

            if(GetAttackResult(btnPos)!=-1) 
            {
				//move successful
				//ship exits
				//ship loc marked as hit
				//Debug.Log ("attack success");
				all_buttons = this.GetComponentsInChildren<Button> ();
				foreach (Button b in all_buttons) {
					if ((b.GetComponent<ButtonInfo> ().position.x == btnPos.x) &&
						(b.GetComponent<ButtonInfo> ().position.y == btnPos.y)) {
				        
                        //checking if location has been already marked as missed or hit
						if ((b.image.color.Equals (Color.black) == false) && (b.image.color.Equals (Color.red) == false)) {

							b.image.color = new Color (Color.red.r, Color.red.g, Color.red.b, 1f);
							hit_Counter++;

					        if (hit_Counter == SgameInfo.max_number_of_hits)
                            {
                                TurnManager.Pwon = true;
								Debug.Log ("game over - player won");
								//winner code - 1 - player won
                                TurnManager.RestartLevel(1);
							}
							Debug.Log("player's turn complete - change turn to PC");
                            TurnManager.EndTurn(false, true);
						}
					}
				}
			} else {
				//get button loc from vector and color grey - no ship
				//Debug.Log ("attack failed");
				all_buttons = this.GetComponentsInChildren<Button> ();
				foreach (Button b in all_buttons) {
					if ((b.GetComponent<ButtonInfo> ().position.x == btnPos.x) &&
						(b.GetComponent<ButtonInfo> ().position.y == btnPos.y)) {
						//checking if location has been already marked as HIT or ship loc marked
						if ((b.image.color.Equals (Color.black) == false) && (b.image.color.Equals (Color.red) == false)) {

							b.image.color = new Color (Color.black.r, Color.black.g, Color.black.b, 1f);
						}
						Debug.Log("player's turn complete - change turn to PC");
                        TurnManager.EndTurn(false, true);
					}
				}
			}
		}
	}

    private int GetAttackResult(Vector2 atck_pos)
    {
        bool attck_result=false;
        int ship_indx=(-1);

        for (int i = 0; i < SgameInfo.max_number_of_ships; i++)
        {
            attck_result = allAIships[i].ifexists(atck_pos);

            if (attck_result)
            {
                ship_indx = i;
            //    Debug.Log("ship hit numebr  " + (i + 1));
                break;
            }
        }
       // Debug.Log("ship indx  " + ship_indx);
        return ship_indx;
    }
}