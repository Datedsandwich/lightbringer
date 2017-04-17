using UnityEngine;
using System.Collections;

public class PlayerBounds : MonoBehaviour 
{
	// Data Members ------------------------------------------
	public enum BoundsBehaviour
	{
		Nothing,
		Constrain,
		Kill
	}

	public BoxCollider2D Bounds;
	public BoundsBehaviour Above;
	public BoundsBehaviour Below;
	public BoundsBehaviour Left;
	public BoundsBehaviour Right;

	private Player _player;
	private BoxCollider2D _boxCollider;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Start()
	{
		_player = GetComponent<Player>();
		_boxCollider = GetComponent<BoxCollider2D>();
	}
	// ------------------------------------------------------
	public void Update()
	{
		if(_player.isDead)
		{
			return;
		}

		var colliderSize = new Vector2(_boxCollider.size.x * Mathf.Abs (transform.localScale.x),
		                               _boxCollider.size.y * Mathf.Abs(transform.localScale.y)) / 2;

		if(Above != BoundsBehaviour.Nothing && transform.position.y + colliderSize.y > Bounds.bounds.max.y)
		{
			ApplyBoundsBehaviour(Above, new Vector2(transform.position.x, Bounds.bounds.max.y - colliderSize.y));
		}

		if(Below != BoundsBehaviour.Nothing && transform.position.y - colliderSize.y < Bounds.bounds.min.y)
		{
			ApplyBoundsBehaviour(Below, new Vector2(transform.position.x, Bounds.bounds.min.y + colliderSize.y));
		}

		if(Right != BoundsBehaviour.Nothing && transform.position.x + colliderSize.x > Bounds.bounds.max.x)
		{
			ApplyBoundsBehaviour(Right, new Vector2(Bounds.bounds.max.x - colliderSize.x, transform.position.y));
		}

		if(Left != BoundsBehaviour.Nothing && transform.position.x - colliderSize.x < Bounds.bounds.min.x)
		{
			ApplyBoundsBehaviour(Left, new Vector2(Bounds.bounds.min.x + colliderSize.x, transform.position.y));
		}
	}
	// -------------------------------------------------------
	private void ApplyBoundsBehaviour(BoundsBehaviour behaviour, Vector2 constrainedPosition)
	{
		if(behaviour == BoundsBehaviour.Kill)
		{
			LevelManager.Instance.KillPlayer();
			return;
		}

		transform.position = constrainedPosition;
	}
	// End Methods -------------------------------------------------------------
}
