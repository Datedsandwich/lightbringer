using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ControllerParameters2D
{
	// Data Members ------------------------------------------
	public enum JumpBehaviour
	{
		canJumpOnGround,
		canJumpAnywhere,
		cantJump
	}
	public Vector2 maxVelocity = new Vector2(float.MaxValue, float.MaxValue);

	[Range(0, 90)]
	public float slopeLimit = 30f;
	public float gravity = -25f;

	public JumpBehaviour jumpRestrictions;
	public float jumpFrequency = 0.25f;
	public float jumpMagnitude = 12f;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
