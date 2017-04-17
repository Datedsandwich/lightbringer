using UnityEngine;
using System.Collections;

public class BackgroundMovement : MonoBehaviour {

	private Transform camera;

	// Use this for initialization
	void Start () 
	{
		camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Move the background to be where the camera is
		transform.position = new Vector3(camera.position.x, camera.position.y, 0f);
	}
}
