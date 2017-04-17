using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public Vector2 offset;
	public Transform following;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Update()
	{
		transform.position = following.transform.position + (Vector3)offset;
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
