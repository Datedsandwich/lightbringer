using UnityEngine;
using System.Collections;

public class ScaleManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		float _selfEsteem = 1 - ((GameManager.Instance.selfEsteem)/100);
		transform.localScale = new Vector3(1.0f, Mathf.Lerp (transform.localScale.y, 1 + _selfEsteem, Time.deltaTime), 1.0f );
	}
}
