﻿using UnityEngine;
using System.Collections;

public class FromWorldPointTextPositioner : IFloatingTextPositioner
{
	// Data Members ------------------------------------------
	private readonly Camera _camera;
	private readonly Vector3 _worldPosition;
	private readonly float _speed;
	private float _timeToLive;

	private float _yOffset;
	// End Data Members --------------------------------------

	// Constructor -------------------------------------------------------------
	public FromWorldPointTextPositioner(Camera camera, Vector3 worldPosition, float timeToLive, float speed)
	{
		_camera = camera;
		_worldPosition = worldPosition;
		_speed = speed;
		_timeToLive = timeToLive;
	}
	// End Constructor ---------------------------------------------------------
	// Methods -----------------------------------------------------------------
	public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
	{
		if((_timeToLive -= Time.deltaTime) <= 0)
		{
			return false;
		}

		var screenPosition = _camera.WorldToScreenPoint(_worldPosition);
		position.x = screenPosition.x - (size.x / 2);
		position.y = Screen.height - screenPosition.y - _yOffset;
		_yOffset += Time.deltaTime * _speed;

		return true;
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
