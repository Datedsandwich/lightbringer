using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public float jumpMagnitude = 20f;
	public AudioClip jumpSound;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void ControllerEnter2D(CharacterController2D controller)
	{
		controller.SetVerticalForce(jumpMagnitude);
		if(jumpSound != null)
		{
			AudioSource.PlayClipAtPoint(jumpSound, transform.position);
		}
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
