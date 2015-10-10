//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;

//This class contains all static valus used for battleship creation, used both in multilplayer and singleplayer modes.
public class SgameInfo : MonoBehaviour
{
    public static int max_Ship_Size;
    public static int max_number_of_ships;
    public static int max_number_of_hits;

    // Use this for initialization
    void Start()
    {
        max_number_of_ships = 4;
        max_Ship_Size = 4;
        max_number_of_hits = 10;
    }
}

