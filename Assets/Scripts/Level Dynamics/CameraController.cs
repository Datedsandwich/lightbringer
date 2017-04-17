using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public Transform playerTransform;
	public Vector2 
		margin,
		smoothing;
	public BoxCollider2D Bounds;

	public bool isFollowing {get; set;}

	private Vector3 
		_min, 
		_max;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Start()
	{
		_min = Bounds.bounds.min;
		_max = Bounds.bounds.max;
		isFollowing = true;
	}
	// -------------------------------------------------------
	public void Update()
	{
		float x = transform.position.x;
		float y = transform.position.y;

		if(isFollowing)
		{
			if(Mathf.Abs (x - playerTransform.position.x) > margin.x)
			{
				x = Mathf.Lerp (x, playerTransform.position.x, smoothing.x * Time.deltaTime);
			}

			if(Mathf.Abs (y - playerTransform.position.y) > margin.y)
			{
				y = Mathf.Lerp (y, playerTransform.position.y, smoothing.y * Time.deltaTime);
			}
		}

		var cameraHalfWidth = GetComponent<Camera>().orthographicSize * ((float)Screen.width / Screen.height);

		x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + GetComponent<Camera>().orthographicSize, _max.y - GetComponent<Camera>().orthographicSize);

		transform.position = new Vector3(x, y, transform.position.z);
	}
	// End Methods -------------------------------------------------------------
}
