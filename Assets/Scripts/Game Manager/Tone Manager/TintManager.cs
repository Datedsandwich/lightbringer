using UnityEngine;
using System.Collections;
/*
 * Summary:
 * This class controls the shifting of the tint for the player character.
 * The Player is a Black and White sprite, this class will shift the white segments
 * from White to Black, gradually, based on the Self Esteem of the Player
 */
public class TintManager : MonoBehaviour 
{
	private float smoothing = 1f;				// to smooth the transition between colours.
	private SpriteRenderer rend;				// The sprite renderer, which is a Unity component, storing the material
	public float minTint = 0.25f;
	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Private self esteem variable, converted from a number into a percentage
		float selfEsteem = Mathf.Max((GameManager.Instance.selfEsteem/100), minTint);
		// Create a new color, with the self esteem as the r, g and b values. Alpha of 1, so it is opaque
		Color newColor = new Color(selfEsteem, selfEsteem, selfEsteem, 1);
		// The object's color, linearly interpolated between it's current color, and the new color. Weighted by the time between frames, multiplied by smoothing
		rend.material.color = Color.Lerp (rend.material.color, newColor, Time.deltaTime * smoothing);
	}
}