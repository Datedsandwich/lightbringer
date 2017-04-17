using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour 
{
	// Data Members ------------------------------------------
	// Used for collisions
	private const float skinWidth = 0.02f;
	private const int totalHorizontalRays = 8;
	private const int totalVerticalRays = 4;
	private GameObject _lastStandingOn;

	private static readonly float slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);		// Slopes are magic
	// but i'll explain it anyway, take the magic away. Take away that child like wonder.
	// 75f is the angle that the player can move on a slope. We need the tangent of it.
	// Unity does not handle degrees when it does quaternions though, so we need to convert it into Radeons.
	// Mathf.Deg2Rad does this.
	public LayerMask platformMask;
	public ControllerParameters2D DefaultParameters;

	public ControllerState2D State {get; private set;}
	public Vector2 velocity {get { return _velocity;}}
	public Vector3 platformVelocity{get; private set;}
	public bool canJump 
	{get 
		{	// Leaving future open for low-gravity levels
			if(Parameters.jumpRestrictions == ControllerParameters2D.JumpBehaviour.canJumpAnywhere)
			{
				return _jumpIn < 0;
			}
			if(Parameters.jumpRestrictions == ControllerParameters2D.JumpBehaviour.canJumpOnGround)
			{
				return State.isGrounded;
			}
			// If there is no jump behaviour set, you can't jump.
			return false;
		}
	}
	public bool HandleCollisions {get; set;}
	public ControllerParameters2D Parameters {get {return _overrideParameters ?? DefaultParameters;}}
	public GameObject standingOn {get; private set;}

	private Vector2 _velocity;
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private ControllerParameters2D _overrideParameters;
	private Vector3 
		_raycastTopLeft,
		_raycastBottomRight,
		_raycastBottomLeft,
		_raycastTopRight;

	private float 
		_verticalDistanceBetweenRays,
		_horizontalDistanceBetweenRays;

	private float _jumpIn;

	private Vector3 _activeGlobalPlatformPoint,
					_activeLocalPlatformPoint;

	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	// -------------------------------------------------------
	public void Awake()
	{
		// If handle collisions is true, we get to run physics code!
		HandleCollisions = true;
		// We need to check the state of the Controller, to check if it's colliding with anything
		State = new ControllerState2D();
		// We store local references to transform, local scale and the collider to save memory
		_transform = transform;
		_localScale = transform.localScale;
		_boxCollider = GetComponent<BoxCollider2D>();
		// The width of the collider, multipled by it's local scale, and with the skinwidth removed (for raycasting)
		var colliderWidth = _boxCollider.size.x * Mathf.Abs (transform.localScale.x) - (2 * skinWidth);
		// The horizontal distance between the rays, we raycast across the entire surface.
		_horizontalDistanceBetweenRays = colliderWidth / (totalVerticalRays - 1);
		// As above, but for vertical raycasting
		var colliderHeight = _boxCollider.size.y * Mathf.Abs (transform.localScale.y) - (2 * skinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (totalHorizontalRays - 1);
	}
	// -------------------------------------------------------
	public void AddForce(Vector2 force)
	{
		// Addition of force to the player
		_velocity += force;
	}
	// -------------------------------------------------------
	public void SetForce(Vector2 force)
	{
		_velocity = force;
	}
	// -------------------------------------------------------
	public void SetHorizontalForce(float x)
	{
		_velocity.x = x;
	}
	// -------------------------------------------------------
	public void SetVerticalForce(float y)
	{
		_velocity.y = y;
	}
	// -------------------------------------------------------
	public void Jump()
	{
		// Add vertical force to the player
		AddForce (new Vector2(0, Parameters.jumpMagnitude));
		// Debug purposes, jumping makes you feel bad
		GameManager.Instance.AddSelfEsteem(25);
		// We just jumped, so we need to grab jumpFrequency to see when we can jump again.
		// This is set to 0 when we hit the ground.
		_jumpIn = Parameters.jumpFrequency;
	}
	// -------------------------------------------------------
	public void LateUpdate()
	{
		_jumpIn -= Time.deltaTime;
		GameManager.Instance.DamageSelfEsteem(5 * Time.deltaTime);
		// LateUpdate runs AFTER Update on all other objects
		// Gravity happens after other behaviours have run
		// Jimmy doesn't wait for gravity. Gravity waits for Jimmy.
		_velocity.y += Parameters.gravity * Time.deltaTime;
		Move (velocity * Time.deltaTime);
	}
	// -------------------------------------------------------
	private void Move(Vector2 deltaMovement)
	{
		// MEAT
		var wasGrounded = State.isCollidingBelow;
		// Obvious, reset State
		State.Reset();
		// If we are handling collisions, so like, if the player is alive
		if(HandleCollisions)
		{
			// handle platforms
			HandlePlatforms();
			// Calculate where we are shooting rays from
			CalculateRayOrigins();
			// If we are not moving vertically and we are/were grounded
			if(deltaMovement.y < 0 && wasGrounded)
			{
				// SLOPES ARE SCARY
				HandleVerticalSlope(ref deltaMovement);
			}
			if(Mathf.Abs (deltaMovement.x) > 0.001f)
			{
				// If we are moving horizontally, literally at all
				MoveHorizontally(ref deltaMovement);
			}
			// Always move vertically, cos gravity is a thing
			MoveVertically(ref deltaMovement);
			CorrectHorizontalPlacement(ref deltaMovement, true);
			CorrectHorizontalPlacement(ref deltaMovement, false);
		}
		// WE DO THE MOVING, in the world space
		_transform.Translate(deltaMovement, Space.World);

		// If the game is running, basically
		if(Time.deltaTime > 0)
		{
			// we multiply by Time.deltaTime to ensure the game runs consistently regardless of frame rate
			_velocity = deltaMovement / Time.deltaTime;
		}
		// Ensure that velocity never exceeds maxVelocity
		_velocity.x = Mathf.Min (_velocity.x, Parameters.maxVelocity.x);
		_velocity.y = Mathf.Min (_velocity.y, Parameters.maxVelocity.y);

		if(State.isMovingUpSlope)
		{
			_velocity.y = 0f;
		}

		// Platform code
		if(standingOn != null)
		{
			// Set the active global and local platform points
			// Global to the players position, local to what we're standing on
			_activeGlobalPlatformPoint = transform.position;
			_activeLocalPlatformPoint = standingOn.transform.InverseTransformPoint(transform.position); // InverseTransformPoint converts world space to local space

			// We've changed what we're standing on
			if(_lastStandingOn != standingOn)
			{
				if(_lastStandingOn != null)
				{
					// Calls the method "ControllerExit2D" on every component of selected game object
					// selected game object is "this"
					// SendMessageOptions.DontRequireReceiver is error checking
					_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
				}
				// Calls the method "ControllerEnter2D" on every component of this game object
				standingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = standingOn;
			}
			else if(standingOn != null) // We haven't changed what we're standing on, and we are standing on something
			{
				standingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if(_lastStandingOn != null)
		{
			_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
			_lastStandingOn = null;
		}
	}
	// -------------------------------------------------------
	private void HandlePlatforms()
	{
		if(standingOn != null)
		{
			// If we're standing on something, we grab the position of the object we're standing on
			var newGlobalPlatformPoint = standingOn.transform.TransformPoint(_activeLocalPlatformPoint);
			// and we get the moveDistance, and stick the player to the platform, essentially
			// This is interesting, because if a platform teleports, the player will STILL be on it
			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
			// As long as moveDistance isn't 0
			if(moveDistance != Vector3.zero)
			{
				// Move the player the same velocity as the platform
				transform.Translate(moveDistance, Space.World);
			}

			platformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
		}

		if(standingOn == null)
		{
			platformVelocity = Vector3.zero;
		}

		standingOn = null;
	}
	// -------------------------------------------------------
	private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
	{
		// We need the center of the player, and the half width so we can raycast to the outside edge of the collider
		var halfWidth = (_boxCollider.size.x * _localScale.x) / 2;
		// raycast stuff, you should know it by now
		var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

		if(isRight)
		{
			rayOrigin.x -= (halfWidth - skinWidth);
		}
		else
		{
			rayOrigin.x += (halfWidth - skinWidth);
		}

		Vector2 rayDirection = isRight ? Vector2.right : -Vector2.right;
		// Offset is how far the player will be pushed
		float offset = 0f;

		for(int i = 1; i < totalHorizontalRays - 1; i++)
		{
			var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
			//Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);
			// RAYCAST
			var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, platformMask);

			if(!raycastHit)
			{
				continue;
			}
			// This is where we work out how far the player will be pushed
			// Offset will equal exactly how far the player got into the platform before the rays detected it
			offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
		}
		// Push the player out of the moving platform. Violently.
		deltaMovement.x += offset;
	}
	// -------------------------------------------------------
	private void CalculateRayOrigins()
	{
		// box collider size * scale, divided by 2
		var size = new Vector2(_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		// Center of the collider
		var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		// raycast origin points, at the corners of the box collider
		_raycastTopLeft = _transform.position + new Vector3(center.x - size.x + skinWidth, center.y + size.y - skinWidth);
		_raycastBottomRight = _transform.position + new Vector3(center.x + size.x - skinWidth, center.y - size.y + skinWidth);
		_raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + skinWidth, center.y - size.y + skinWidth);
		_raycastTopRight = _transform.position + new Vector3(center.x + size.x - skinWidth, center.y + size.y - skinWidth);
	}
	// -------------------------------------------------------
	private void MoveHorizontally(ref Vector2 deltaMovement)
	{
		// isGoingRight is true, if we are going right! omg!
		bool isGoingRight = deltaMovement.x > 0;
		// How far are we raycasting?
		float rayDistance = Mathf.Abs (deltaMovement.x) + skinWidth;
		// The direction of the raycast must be the same as the direction the player is moving
		Vector2 rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		// Where are we firing the rays from?
		Vector3 rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

		for (int i = 0; i < totalHorizontalRays; i++)
		{
			// RAYCASTING! WOO! THIS IS FUN!
			// make a vector for the raycast
			Vector2 rayVector = new Vector2(rayOrigin.x, rayOrigin.y + i * _verticalDistanceBetweenRays);
			// Draw the ray in the editor
			Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
			// Log to make sure we are actually raycasting
			//Debug.Log("ASDF");
			// RAYCAST!
			RaycastHit2D raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, platformMask);
			if(!raycastHit)
			{
				// if the raycast didn't hit anything, skip the rest of the code in the loop and go to the next iteration
				// We don't use break here, because we want the loop to keep going
				continue;
			}
			// funky slope stuff
			if( i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle (raycastHit.normal, Vector2.up), isGoingRight))
			{
				break;
			}
			// STOP IN THE NAME OF THE LAW!
			deltaMovement.x = raycastHit.point.x - rayVector.x;
			// If the ray hit something, there is no point raycasting further than what it hit, it already hit something
			rayDistance = Mathf.Abs (deltaMovement.x);
			// if we go right, we tell the State that it is colliding right
			if(isGoingRight)
			{
				// -skinWidth cos it's inside the right side of the player, so it's like a right offset, it goes left
				deltaMovement.x -= skinWidth;
				State.isCollidingRight = true;
			}
			else
			{
				// and vice versa
				deltaMovement.x += skinWidth;
				State.isCollidingLeft = true;
			}
			// If we're raycasting inside the object, just stop
			if(rayDistance < skinWidth + 0.0001f)
			{
				break;
			}
		}
	}
	// -------------------------------------------------------
	private void MoveVertically(ref Vector2 deltaMovement)
	{
		// we going up, yo?
		bool isGoingUp = deltaMovement.y > 0;
		float rayDistance = Mathf.Abs (deltaMovement.y) + skinWidth;
		Vector2 rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		Vector3 rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;
		// shoot our rays as an offset where we want to go on the x axis
		// this gives us a value to where we are going
		rayOrigin.x += deltaMovement.x;

		float standingOnDistance = float.MaxValue;

		for(int i = 0; i < totalVerticalRays; i++)
		{
			// As with horizontal movement, we need a direction vector for the ray
			Vector2 rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
			Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
			//RAYCAST!
			var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, platformMask);

			if(!raycastHit)
			{
				// If the raycast didn't hit anything, next iteration
				continue;
			}
			if(!isGoingUp)
			{
				// If we're going down
				// The vertical distance to hit something is our current position - the raycast point
				var verticalDistanceToHit = transform.position.y - raycastHit.point.y;
				// if we have collided, basically
				if(verticalDistanceToHit < standingOnDistance)
				{
					// This is used for moving platforms, to move the player with the platform.
					standingOnDistance = verticalDistanceToHit;
					standingOn = raycastHit.collider.gameObject;
				}
			}
			// This is clever code right here
			// This will stop us moving on the y axis if we hit something
			// but if it returns a negative value, say, we're stuck inside of a piece of level geometry
			// it will push us out of the colliding object!
			deltaMovement.y = raycastHit.point.y - rayVector.y;
			// No point casting further than we've already hit
			rayDistance = Mathf.Abs(deltaMovement.y);

			if(isGoingUp)
			{
				// if we're going up, - skinWidth from deltaMovment
				deltaMovement.y -= skinWidth;
				State.isCollidingAbove = true;
			}
			else
			{
				deltaMovement.y += skinWidth;
				State.isCollidingBelow = true;
			}
			// if we're not going up and deltaMovement.y is greater than 0, we're going up a slope
			if(!isGoingUp && deltaMovement.y > 0.0001f)
			{
				State.isMovingUpSlope = true;
			}
		}
	}
	// -------------------------------------------------------
	private void HandleVerticalSlope(ref Vector2 deltaMovement)
	{
		// Let's get the center of our player collider
		var center = (_raycastBottomLeft.x + _raycastBottomRight.x)/2;
		// The direction we're going to be raycasting
		var direction = -Vector2.up;	// Down
		// The distance of the slope. slopeLimitTangent is magic.
		var slopeDistance = slopeLimitTangent * (_raycastBottomRight.x - center);
		// The vector for the raycast
		var slopeRayVector = new Vector2(center, _raycastBottomLeft.y);
		// Draw the ray in the editor
		Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.yellow);
		// RAYCAST!
		RaycastHit2D raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, platformMask);

		if(!raycastHit)
		{
			// If we don't hit anything, return
			return;
		}
		// Are we moving down a slope?
		// Mathf.Sign() will return 1 if the value is positive, -1 if it's negative, or 0 if it's 0
		var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign (deltaMovement.x);

		if(!isMovingDownSlope)
		{
			// if not, return
			return;
		}
		// Let's store the angle of the slope we're on
		var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
		if(Mathf.Abs (angle) < 0.001f)
		{
			// If angle is smaller than 0.001f, we're not on a slope, it shouldn't have gotten this far
			// oh well, return
			return;
		}
		// STATES
		State.isMovingDownSlope = true;
		State.slopeAngle = angle;
		deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
	}
	// -------------------------------------------------------
	private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
	{
		// This is where the magic happens, mhmm
		// if angle is 90, we're not on a fuckin slope
		if(Mathf.RoundToInt(angle) == 90)
		{
			return false;
		}
		// if the slope is too steep, just stop
		if(angle > Parameters.slopeLimit)
		{
			deltaMovement.x = 0f;
			return true;
		}
		// if the y velocity is greater than 0.07f we're on a slope woo
		if(deltaMovement.y > 0.07f)
		{
			return true;
		}
		// if we're going right, we need to subtract our skinWidth from deltaMovement.x and vice versa
		deltaMovement.x += isGoingRight ? -skinWidth : +skinWidth;
		// Our y movement is complex. We need the absolute value of the Tangent of Angle (converted to Radeons) multipled by our x movement
		// This makes it so we go slower, the steeper the slope :D
		deltaMovement.y = Mathf.Abs (Mathf.Tan (angle * Mathf.Deg2Rad) * deltaMovement.x);
		State.isMovingUpSlope = true;
		State.isCollidingBelow = true;
		return true;
	}
	// -------------------------------------------------------
	public void OnTriggerEnter2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
		if(parameters == null)
		{
			return;
		}
		else
		{
			_overrideParameters = parameters.Parameters;
		}
	}
	// -------------------------------------------------------
	public void OnTriggerExit2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
		if(parameters == null)
		{
			return;
		}
		else
		{
			_overrideParameters = null;
		}
	}
	// End Methods -------------------------------------------------------------
}
