//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script
using System.Collections;

public class IntroUI : MonoBehaviour
{
    private string intro_msg;
    public GameObject Intro_panel;
    public Text instrcs;

    void Start()
    {
        intro_msg = "1. Place " + SgameInfo.max_number_of_ships + " battleships on your grid, when the largset ship size is of "
                    + SgameInfo.max_Ship_Size + " squares.\n" + "The first ship is the largset, the second one will be a square size less and so on\n\n";

        intro_msg = intro_msg + "2. Once all ships are placed, your opponent will be allowed to make a move and try to hit one of your ships.\n" +
                    "A SUCCESSFUL hit will be marked in RED and a MISS will be marked in BLACK.\n\n";

        intro_msg = intro_msg + "3. The game will end when either all player nor enemy ships will be destroyed.\n\n";

        instrcs.text = intro_msg.ToString();

        this.StartCoroutine(Wait());
    }

    //coroutine to wait 10 seconds before starting
    private IEnumerator Wait()
    {
        Debug.Log("waiting for 7 seconds before starting the level");
        yield return new WaitForSeconds(7.0f);
        Intro_panel.SetActive(false);
        Debug.Log("finished waiting for 7 seconds before starting the level");
    }
}
