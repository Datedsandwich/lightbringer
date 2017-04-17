using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	// Data Members ------------------------------------------
	private bool _isFacingRight;
	private CharacterController2D _controller;
	private float _normalisedHorizontalSpeed;
	private float _canFireIn;

	public float maxSpeed = 8f;
	public float speedAccelerationOnGround = 10f;
	public float speedAccelerationInAir = 5f;

	public bool isDead {get; private set;}

	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Awake()
	{
		_controller = GetComponent<CharacterController2D>();
		_isFacingRight = transform.localScale.x > 0;
	}
	// -------------------------------------------------------
	public void Update()
	{
		_canFireIn -= Time.deltaTime;
		// We do not handle input if the player is dead
		if(!isDead)
		{
			// Honestly this code could probably be put in another object, keeping with single responsibility principle
			// That's more work that we don't really have time for though
			HandleInput();
		}

		var movementFactor = _controller.State.isGrounded ? speedAccelerationOnGround : speedAccelerationInAir;

		if(isDead)
		{
			// We don't move if we're dead
			_controller.SetHorizontalForce(0);
		}
		else
		{
			// This is the code that actually moves the player
			// We use Mathf.Lerp to make it a smooth transition, like the player accelerates, rather than just zipping off
			// _normalisedHorizontalSpeed is set in HandleInput()
			_controller.SetHorizontalForce(Mathf.Lerp (_controller.velocity.x, _normalisedHorizontalSpeed * maxSpeed, Time.deltaTime * movementFactor));
		}
	}
	// -------------------------------------------------------
	public void FinishLevel()
	{
		enabled = false;
		_controller.enabled = false;
		GetComponent<Collider2D>().enabled = false;
	}
	// -------------------------------------------------------
	public void Kill()
	{
		// No more collisions, we ded
		_controller.HandleCollisions = false;
		GetComponent<Collider2D>().enabled = false;
		// we ded
		isDead = true;
		// Sonic The Hedgehog death effect!
		_controller.SetForce(new Vector2(0, 10));
	}
	// -------------------------------------------------------
	public void RespawnAt(Transform spawnPoint)
	{
		// If we aren't facing right when spawning, we need to change that
		if(!_isFacingRight)
		{
			// omg I wonder what this does
			Flip ();
		}
		// we no ded, collishuns happen
		isDead = false;
		GetComponent<Collider2D>().enabled = true;
		_controller.HandleCollisions = true;
		// Spawn! Player is never destroyed, only moved
		transform.position = spawnPoint.position;
	}
	// -------------------------------------------------------
	private void HandleInput()
	{
		// Movement keys, used to be hardcoded as WASD
		// Updated to use Axis system in Unity, which allows for player editing of input keys
		// Input.GetAxis() is a float between -1 and 1
		_normalisedHorizontalSpeed = Input.GetAxis("Horizontal");
		if(_normalisedHorizontalSpeed < 0 && _isFacingRight)
		{
			Flip();
		}
		else if(_normalisedHorizontalSpeed > 0 && !_isFacingRight)
		{
			Flip();
		}

		// GetAxis("Jump") will be 0 if not pressed, 1 if pressed
		if(_controller.canJump && Input.GetAxis("Jump") == 1)
		{
			_controller.Jump();
		}
	}
	// -------------------------------------------------------
	private void Flip()
	{
		// Flippity floop on the x axis
		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		// Then we set whether or not we are facing right
		_isFacingRight = transform.localScale.x > 0;
	}
	// -------------------------------------------------------
	// End Methods -------------------------------------------------------------
}
