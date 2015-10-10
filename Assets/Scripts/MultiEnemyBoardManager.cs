//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Operation of the opponent's game board.
//At initialization the second player places the battleships and is used by the MultiplayerBoardManager
//in order to make a game move upon button press.
//either hitting or missing the opponent's battleship.
public class MultiEnemyBoardManager : MonoBehaviour
{
    private Button[] all_buttons;

    private GameObject appwarp_logic;
    private SC_Logic appwarp_logic_sc;

    private int enemy_hit_Counter;

	void Start ()
	{
		Debug.Log ("start in multi ebm");
        appwarp_logic = GameObject.Find("NetworkManager");
        appwarp_logic_sc = appwarp_logic.GetComponent<SC_Logic>();
        enemy_hit_Counter = 0;
        Debug.Log("done start in multi ebm");
	}

    public void OnButtonPressed(Button btn)
    {
		Debug.Log ("in OnButtonPressed multi ebm");

        string str_move_vec="";

		if (appwarp_logic_sc.IsItMine()) {

			Vector2 btnPos = btn.GetComponent<ButtonInfo> ().position;

			Debug.Log ("vector x " + btnPos.x + "vector y " + btnPos.y);
			//checking if boat structure is not complete yet and if it's the player's turn to attack
            str_move_vec = "{X:" + btnPos.x + "Y:" + btnPos.y + "}";
            appwarp_logic_sc.MakeMyMove(str_move_vec);
		}
	}

    public void MarkAttackResult(Vector2 vc)
    {
        //attack was success
        if (appwarp_logic_sc.my_enemy_attack_res == 1)
        {
            all_buttons = this.GetComponentsInChildren<Button>();
            foreach (Button b in all_buttons)
            {
                if ((b.GetComponent<ButtonInfo>().position.x == vc.x) &&
               (b.GetComponent<ButtonInfo>().position.y == vc.y))
                {
                    //checking if location has been already marked as missed
                    if ((b.image.color.Equals(Color.red) == false) && (b.image.color.Equals(Color.green) == false))
                    {
                        //mark location as hit
                        b.image.color = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
                        enemy_hit_Counter++;
                        
                        if (enemy_hit_Counter == SgameInfo.max_number_of_hits)
                        {
                            Debug.Log("game over - U won");
                            ConnStater.set_Game_Result(1);
                        }
                    }
                }
            }
        }
        else
        {
            //attack failed
            if (appwarp_logic_sc.my_enemy_attack_res == 0)
            {
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
                            b.image.color = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
                        }
                    }
                }
            }
        }
    }
}