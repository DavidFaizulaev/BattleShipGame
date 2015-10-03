using UnityEngine;
using System.Collections;

//class validates input both user and AI. To make sure that ship creation is done correctly.
public class InputValidator
{
    private Vector2 initial_vc;

    private bool used;

    private int[] min_max_x;
    private int[] min_max_y;

    public void InitValidator()
    {
        Debug.Log("in InputValidator Start");
        used = false;
        min_max_x = new int[2];
        min_max_y = new int[2];
        min_max_x[0] = min_max_x[1] = min_max_y[0] = min_max_y[1] = 0;
        Debug.Log("out InputValidator Start");
    }

    public bool Get_used()
    {
        return used;
    }

    public void InitFirstVctr(Vector2 vec)
    {
        used = true;
        initial_vc = new Vector2(vec.x, vec.y);

        //set potential min max values for X axis
        //set potential X min value
        if (initial_vc.x < 3)
        {
            min_max_x[0] = 0;
        }
        else
        {
            min_max_x[0] = (int)initial_vc.x - 3;
        }

        //set potential Y max value
        if (initial_vc.x > 6)
        {
            min_max_x[1] = 9;
        }
        else
        {
            min_max_x[1] = (int)initial_vc.x + 3;
        }

        //set potential min max values for Y axis
        //set potential Y min value
        if (initial_vc.y < 3)
        {
            min_max_y[0] = 0;
        }
        else
        {
            min_max_y[0] = (int)initial_vc.y - 3;
        }

        //set potential Y max value
        if (initial_vc.y > 6)
        {
            min_max_y[1] = 9;
        }
        else
        {
            min_max_y[1] = (int)initial_vc.y + 3;
        }
       // Debug.Log("successfully initialized first valid vector location");
    }

    //method checks if received vector values hold the min/max X & min/max Y values
    public bool CheclVectorValues(Vector2 vc)
    {
        bool res_val = false;

        if ((vc.x == initial_vc.x) || (vc.y == initial_vc.y))
        {
            if (vc.x == initial_vc.x)
            {
                if (((int)vc.y <= min_max_y[1]) && (((int)vc.y >= min_max_y[0])))
                {
                    Update_MinMax(vc);
                //    Debug.Log("vector location valid");
                    res_val = true;
                }
            }
            else
            {
                if (((int)vc.x <= min_max_x[1]) && ((int)vc.x >= min_max_x[0]))
                {
                    Update_MinMax(vc);
                //    Debug.Log("vector location valid");
                    res_val = true;
                }
            }
        }

        return res_val;
    }

    private void Update_MinMax(Vector2 vc)
    {
        //set potential min max values for X axis
        //set potential X min value
        if (vc.x < 3)
        {
            min_max_x[0] = 0;
        }
        else
        {
            min_max_x[0] = (int)vc.x - 3;
        }

        //set potential X max value
        if (vc.x > 6)
        {
            min_max_x[1] = 9;
        }
        else
        {
            min_max_x[1] = (int)vc.x + 3;
        }

        //set potential Y min value
        if (vc.y < 3)
        {
            min_max_y[0] = 0;
        }
        else
        {
            min_max_y[0] = (int)vc.y - 3;
        }

        //set potential Y max value
        if (vc.y > 6)
        {
            min_max_y[1] = 9;
        }
        else
        {
            min_max_y[1] = (int)vc.y + 3;
        }
    }
}