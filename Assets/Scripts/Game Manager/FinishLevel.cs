using UnityEngine;
using System.Collections;

public class FinishLevel : MonoBehaviour {

	// Data Members ------------------------------------------
	public string LevelName;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponent<Player>() == null)
		{
			return;
		}

		LevelManager.Instance.GotoNextLevel(LevelName);
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
