using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public enum FollowType
	{
		MoveTowards,
		Lerp
	}

	public FollowType type = FollowType.MoveTowards;
	public PathDefinition path;
	public float speed = 1f;
	public float MaxDistanceToGoal = 0.1f;

	private IEnumerator<Transform> _currentPoint;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Start()
	{
		if(path == null)
		{
			Debug.LogError ("Path cannot be null", gameObject);
			return;
		}

		_currentPoint = path.GetPathsEnumerator();
		_currentPoint.MoveNext();

		if(_currentPoint.Current == null)
		{
			// return, there is no point
			return;
		}
		// Platform starts on first point in the path
		transform.position = _currentPoint.Current.position;
	}
	// -------------------------------------------------------
	public void Update()
	{
		if(_currentPoint == null || _currentPoint.Current == null)
		{
			return;
		}

		if(type == FollowType.MoveTowards)
		{
			// Built in Unity function. MoveTowards is kind of jerky
			transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
		}
		else if(type == FollowType.Lerp)
		{
			// whereas Lerp is smooth, but somewhat slower
			transform.position = Vector3.Lerp (transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
		}

		float distanceSquared = (transform.position - _currentPoint.Current.position).sqrMagnitude;
		if(distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal)
		{
			_currentPoint.MoveNext();
		}
	}
	// End Methods -------------------------------------------------------------
}
