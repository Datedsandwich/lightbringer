using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour 
{
	// Data Members ------------------------------------------
	// A list of objects that have behaviour for when the player respawns
	private List<IPlayerRespawnListener> _listeners;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Awake()
	{
		_listeners = new List<IPlayerRespawnListener>();
	}
	public void PlayerLeftCheckpoint()
	{

	}
	// -------------------------------------------------------
	public void SpawnPlayer(Player player)
	{
		player.RespawnAt(transform);

		foreach(var listener in _listeners)
		{
			// Perform behaviour on all objects assigned to this checkpoint
			listener.OnPlayerRespawnInThisCheckpoint(this, player);
		}
	}
	// -------------------------------------------------------
	public void AssignObjectToCheckpoint(IPlayerRespawnListener listener)
	{
		_listeners.Add (listener);
	}
	// End Methods -------------------------------------------------------------
}
