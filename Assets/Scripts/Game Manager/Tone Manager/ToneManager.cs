using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class ToneManager : MonoBehaviour 
{
	// grayscale script reference
	private Grayscale grayScale;
	public float smoothing = 1f;			// To smooth transition

	// Use this for initialization
	void Start () 
	{
		// Set up reference
		grayScale = GetComponent<Grayscale>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateGrayScale();
	}

	void UpdateGrayScale()
	{
		// Convert self esteem into a percentage. Not strictly necessary, helps for readability.
		float selfEsteem = 1 - (GameManager.Instance.selfEsteem/100);
		// Set the effectAmount variable in grayScale to a linearly interpolated value, between it's current value and selfesteem
		grayScale.effectAmount = Mathf.Lerp (grayScale.effectAmount, selfEsteem, Time.deltaTime * smoothing);
	}
}
