using UnityEngine;

public interface IFloatingTextPositioner
{
	// Data Members ------------------------------------------
	
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size);
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
