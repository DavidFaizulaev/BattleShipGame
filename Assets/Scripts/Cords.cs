//Unity course Summer 2015 - David Faizulaev
using UnityEngine;
using System.Collections;

//The class is used to mark a battleship's cordinatnes, via X,Y axis in each button
public class Cords
{
	private bool hitmiss;//true - hit, false - no hit
	private Vector2 Loc;

	public Cords (Vector2 v)
	{
		hitmiss = false;
		Loc = v;
	}

	public Vector2 GetCordLoc ()
	{
		return Loc;
	}

	public bool GetState ()
	{
		return hitmiss;
	}

	public void Register_Hit()
	{
		hitmiss = true;
	}

}