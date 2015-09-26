//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;

//This class operats the battleship structure and 'places' the selected location in the battleship structure.
public class BattleShip
{
    private Cords[] ship_conf;
    private int ship_size;
    private int cur_loc;
    private bool ship_status;
    private bool structure_complete;

    private InputValidator input_validator;

    //initialize battleship parameteres
    public void Init_Ship(int s_size)
    {
        Debug.Log("ship class init start");

        ship_conf = new Cords[s_size];
        ship_size = s_size;
        cur_loc = 0;
        ship_status = true;//alive
        structure_complete = false; //no ship yet

        input_validator = new InputValidator();
        input_validator.InitValidator();

        Debug.Log("ship class init done");
    }

    public void Set_ShipStatus(bool val)
    {
        ship_status = val;
    }

    public bool Get_ShipStatus()
    {
        return ship_status;
    }

    public bool Getstructure_state()
    {
        return structure_complete;
    }

    //placing a ship square in the received Vector 2 if ship does not exist in that location yet.
    public bool Set_Loc(Vector2 vc)
    {
        Debug.Log("in set loc");
        bool s = true;
        bool res_val = false;
        bool check_res = false;

        if ((cur_loc != ship_size) && (!structure_complete))
        {
            for (int i = 0; i < cur_loc; i++)
            {
                if ((ship_conf[i].GetCordLoc().x == vc.x) && (ship_conf[i].GetCordLoc().y == vc.y))
                {
                    s = false;
                    Debug.Log("ship in loc already exists");
                    res_val = false;
                }
            }

            if (s == true)
            {
                if (input_validator.Get_used() == false)
                {
                    input_validator.InitFirstVctr(vc);

                   ship_conf[cur_loc] = new Cords(vc);
                   cur_loc++;

                    if (cur_loc == ship_size)
                    { structure_complete = true; } //ship complete

                    Debug.Log("new loc added to ship");
                    res_val = true;
                }

                else
                {
                    check_res = input_validator.CheclVectorValues(vc);

                    if (check_res == true)
                    {
                        ship_conf[cur_loc] = new Cords(vc);
                        cur_loc++;

                        if (cur_loc == ship_size)
                        { structure_complete = true; } //ship complete

                        Debug.Log("new loc added to ship");
                        res_val = true;
                    }

                    else
                    {
                        Debug.Log("loc out of bounds");
                        res_val = false;
                    }
                }    
            }
        }
        else
        {
            Debug.Log("ship already complete");
            res_val = false;
        }

        return res_val;
    }

    //checking if ship is location at the recieved Vector2
    public bool ifexists(Vector2 vc)
    {
        if (ship_status == true)
        {
            for (int i = 0; i < cur_loc; i++)
            {
                if (((ship_conf[i].GetCordLoc().x == vc.x) && (ship_conf[i].GetCordLoc().y == vc.y) && ship_conf[i].GetState() == false))
                {
                    Debug.Log("ship loc confirmed");
                    ship_conf[i].Register_Hit();
                    return true; //hit made
                }
            }
            Debug.Log("ship loc not good");
            return false;
        }
        else
        {
            Debug.Log("ship loc not good");
            return false;
        }
    }

    public bool IsLocAvailable(Vector2 vc)
    {
        for (int i = 0; i < cur_loc; i++)
        {
            if ((ship_conf[i].GetCordLoc().x == vc.x) && (ship_conf[i].GetCordLoc().y == vc.y))
            {
               // Debug.Log("ship loc confirmed");
               // ship_conf[i].Register_Hit();
                return false; //hit made
            }
        }
        return true;
    }
}