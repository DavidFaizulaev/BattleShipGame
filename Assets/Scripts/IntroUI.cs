//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script
using System.Collections;

// UI aid class to display the game instructions before the game begins.
public class IntroUI : MonoBehaviour
{
    private string intro_msg;
    public GameObject Intro_panel;
    public Button exit_btn;
    public Text instrcs;
    
    void Start()
    {
        intro_msg = "1. Place " + SgameInfo.max_number_of_ships + " battleships on your grid, when the largset ship size is of "
                    + SgameInfo.max_Ship_Size + " squares.\n" + "\t\tThe first ship is the largset, the second one will be a square size less and so on\n\n";

        intro_msg = intro_msg + "2. Once all ships are placed, your opponent will be allowed to make a move and try to hit one of your ships.\n" +
                    "\t\t\tA SUCCESSFUL hit will be marked in RED and a MISS will be marked in BLACK.\n\n";

        intro_msg = intro_msg + "3. The game will end when either all player nor enemy ships will be destroyed.\n\n";

        intro_msg = intro_msg + "\tInstructions will disappear after 10 seconds\n";

        instrcs.text = intro_msg.ToString();

        exit_btn.enabled = false;

        this.StartCoroutine(Wait());
    }

    //coroutine to wait 7 seconds before starting
    private IEnumerator Wait()
    {
     //   Debug.Log("waiting for 12 seconds before starting the level");
        yield return new WaitForSeconds(0.0f);
        Intro_panel.SetActive(false);
        exit_btn.enabled = true;
        //Debug.Log("finished waiting for 7 seconds before starting the level");
    }

    public void QuitGamePress() //This function will be used on our "Exit Game" button
    {
        Debug.Log("ExitGamePress");
        Application.Quit(); //This will quit our game.  
    }
}
