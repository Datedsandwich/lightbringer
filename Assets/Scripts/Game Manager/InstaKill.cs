using UnityEngine;
using System.Collections;

public class InstaKill : MonoBehaviour 
{
	// Data Members ------------------------------------------
	
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.GetComponent<Player>();
		if(player == null)
		{
			return;
		}
		else
		{
			LevelManager.Instance.KillPlayer();
		}
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
