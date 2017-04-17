using UnityEngine;
using System.Collections;

public class ControllerState2D
{
	// Data Members ------------------------------------------
	public bool isCollidingRight{get; set;}
	public bool isCollidingLeft{get; set;}
	public bool isCollidingAbove{get; set;}
	public bool isCollidingBelow{get; set;}
	public bool isMovingDownSlope{get; set;}
	public bool isMovingUpSlope{get; set;}
	public bool isGrounded{get {return isCollidingBelow;}}
	public float slopeAngle{get; set;}

	public bool hasCollisions{ get {return isCollidingLeft || isCollidingRight || isCollidingAbove || isCollidingBelow;}}
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Reset()
	{
		isMovingUpSlope = 
			isMovingDownSlope =
			isCollidingLeft =
			isCollidingRight =
			isCollidingAbove =
			isCollidingBelow = false;

		slopeAngle = 0f;
	}
	// -------------------------------------------------------
	public override string ToString ()
	{
		return string.Format ("[ControllerState2D: R={0}, L={1}, A={2}, B={3}, DownSlope={4}, UpSlope={5}, slopeAngle={6}, hasCollisions={7}]", isCollidingRight, isCollidingLeft, isCollidingAbove, isCollidingBelow, isMovingDownSlope, isMovingUpSlope, slopeAngle, hasCollisions);
	}
	// End Methods -------------------------------------------------------------
}
