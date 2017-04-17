using UnityEngine;

public class CenteredTextPositioner : IFloatingTextPositioner
{
	// Data Members ------------------------------------------
	private readonly float _speed;
	private float _textPosition;
	// End Data Members --------------------------------------

	// Constructor -------------------------------------------------------------
	public CenteredTextPositioner(float speed)
	{
		_speed = speed;
	}
	// End Constructor ---------------------------------------------------------

	// Methods -----------------------------------------------------------------
	public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
	{
		_textPosition += Time.deltaTime * _speed;
		if(_textPosition > 1)
		{
			return false;
		}

		position = new Vector2(Screen.width / 2f - size.x / 2f, Mathf.Lerp (Screen.height / 2f + size.y, 0, _textPosition));
		return true;
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
