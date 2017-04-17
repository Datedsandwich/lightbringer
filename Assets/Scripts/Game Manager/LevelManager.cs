using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public static LevelManager Instance {get; private set;}
	public Player player {get; private set;}
	public CameraController camera {get; private set;}
	public TimeSpan RunningTime {get {return DateTime.UtcNow - _started;}}

	private List<Checkpoint> _checkpoints;
	private int _currentCheckpointIndex;
	private DateTime _started;

	public Checkpoint debugSpawn;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Awake()
	{
		Instance = this;
	}
	// -------------------------------------------------------
	public void Start()
	{
		// _checkpoints is a Checkpoint List object, ordered by position
		_checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
		// If there are checkpoints in the level, set the current index to 0, otherwise use -1
		_currentCheckpointIndex = _checkpoints.Count > 1 ? 0 : -1;

		player = FindObjectOfType<Player>();
		camera = FindObjectOfType<CameraController>();

		_started = DateTime.UtcNow;

		var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
		foreach(var listener in listeners)
		{
			for(var i = _checkpoints.Count - 1; i >= 0; i--)
			{
				var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints[i].transform.position.x;
				if(distance < 0)
				{
					continue;
				}
				
				_checkpoints[i].AssignObjectToCheckpoint(listener);
				break;
			}
		}

		if(debugSpawn != null)
		{
			debugSpawn.SpawnPlayer(player);
		}
		else if(_currentCheckpointIndex != -1)
		{
			_checkpoints[_currentCheckpointIndex].SpawnPlayer (player);
		}
	}
	// -------------------------------------------------------
	public void Update()
	{
		bool isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
		if(isAtLastCheckpoint)
		{
			return;
		}

		var distanceToNextCheckpoint = _checkpoints[_currentCheckpointIndex + 1].transform.position.x - player.transform.position.x;

		if(distanceToNextCheckpoint >= 0)
		{
			return;
		}
		// Player has left a checkpoint
		_checkpoints[_currentCheckpointIndex].PlayerLeftCheckpoint();
		// Increment checkpoint index
		_currentCheckpointIndex++;
		_started = DateTime.UtcNow;
	}
	// -------------------------------------------------------
	public void KillPlayer()
	{
		StartCoroutine(KillPlayerCo ());
	}
	// -------------------------------------------------------
	private IEnumerator KillPlayerCo()
	{
		player.Kill();
		camera.isFollowing = false;
		yield return new WaitForSeconds(2f);

		camera.isFollowing = true;
		if(_currentCheckpointIndex != -1)
		{
			_checkpoints[_currentCheckpointIndex].SpawnPlayer(player);
		}

		_started = DateTime.UtcNow;
	}
	// -------------------------------------------------------
	public void GotoNextLevel(string levelName)
	{
		StartCoroutine(GotoNextLevelCo(levelName));
	}
	// -------------------------------------------------------
	private IEnumerator GotoNextLevelCo(string levelName)
	{
		player.FinishLevel();
		FloatingText.Show (string.Format("Level Complete!"), "CheckpointText", new CenteredTextPositioner(0.25f));
		yield return new WaitForSeconds(1f);
		yield return new WaitForSeconds(2f);

		if(string.IsNullOrEmpty(levelName))
		{
			Application.LoadLevel("start_screen");
		}
		else
		{
			Application.LoadLevel(levelName);
		}
	}
	// End Methods -------------------------------------------------------------
}
