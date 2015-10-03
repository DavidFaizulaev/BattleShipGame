using UnityEngine;
using System.Collections;

public class ConnStater : MonoBehaviour {

    private static bool login_ready;
    private static bool connection_status;
    private static string usrnm;

	// Use this for initialization
	void Start () 
    {
        login_ready = false;
        connection_status = false;
        usrnm = "";
	}
	
    public static void Set_login_ready (bool state,string usr_name)
    {
        login_ready = state;
        usrnm = usr_name;
    }

    public static bool Get_login_ready()
    {
        return login_ready;
    }

    public static bool Get_connection_status()
    {
        return connection_status;
    }

    public static void Set__connection_status(bool state)
    {
        connection_status = state;
    }

    public static string get_Username()
    {
        return usrnm;
    }
}
