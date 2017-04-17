using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class PathDefinition : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public Transform[] points;
	// End Data Members --------------------------------------

	// Methods -----------------------------------------------------------------
	public IEnumerator<Transform> GetPathsEnumerator()
	{
		if(points == null || points.Length < 1)
		{
			// Terminate the sequence.
			// Cannot use return in an Enumerator
			yield break;
		}

		int direction = 1;
		int index = 0;
		// Infinite loop here we come
		while(true)
		{
			// Without this, the program will crash.
			// Yield will Yield execution to the object running this enumerator
			// until it is called again
			// So it's not REALLY an infinite loop, if the object stops looking for the next point
			// this will terminate
			yield return points[index];

			if(points.Length == 1)
			{
				continue;
			}
			if(index <= 0)
			{
				// Move towards next point
				direction = 1;
			}
			else if (index >= points.Length - 1)
			{
				// move towards previous point
				direction = -1;
			}

			index = index + direction;
		}
	}
	// -------------------------------------------------------
	public void OnDrawGizmos()
	{
		// This function runs in the Unity Editor, and draws the path in the Scene View.
		if(points == null || points.Length < 2)
		{
			// Make sure we have enough points to actually make a line, otherwise return
			return;
		}
		var Points = points.Where(t => t != null).ToList();
		if(Points.Count < 2)
		{
			return;
		}
		// Draw that line
		for(int i = 1; i < Points.Count; i++)
		{
			Gizmos.DrawLine(Points[i - 1].position, Points[i].position);
		}
	}
	// End Methods -------------------------------------------------------------
}
