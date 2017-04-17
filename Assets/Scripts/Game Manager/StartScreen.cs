using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public string firstLevel;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Update()
	{
		if(!Input.GetMouseButtonDown(0))
		{
			return;
		}
		Application.LoadLevel(firstLevel);
	}
	// -------------------------------------------------------

	// End Methods -------------------------------------------------------------
}
